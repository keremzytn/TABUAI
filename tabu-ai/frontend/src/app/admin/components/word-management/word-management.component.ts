import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../services/admin.service';
import { Word } from '../../../models/game.models';
import { ToastService } from '../../../services/toast.service';

@Component({
    selector: 'app-word-management',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './word-management.component.html',
    styleUrls: ['./word-management.component.css']
})
export class WordManagementComponent implements OnInit {
    words: Word[] = [];
    isLoading = false;
    isModalOpen = false;

    currentWord: Word = this.getEmptyWord();
    tabuWordsString = '';

    constructor(
        private adminService: AdminService,
        private toastService: ToastService
    ) { }

    ngOnInit(): void {
        this.loadWords();
    }

    loadWords(): void {
        this.isLoading = true;
        this.adminService.getAllWords().subscribe({
            next: (words) => {
                this.words = words;
                this.isLoading = false;
            },
            error: () => {
                this.toastService.error('Kelimeler yüklenirken bir hata oluştu.');
                this.isLoading = false;
            }
        });
    }

    openModal(word?: Word): void {
        if (word) {
            this.currentWord = { ...word, tabuWords: [...word.tabuWords] };
            this.tabuWordsString = word.tabuWords.join(', ');
        } else {
            this.currentWord = this.getEmptyWord();
            this.tabuWordsString = '';
        }
        this.isModalOpen = true;
    }

    closeModal(): void {
        this.isModalOpen = false;
    }

    saveWord(): void {
        // Parse tabu words string
        this.currentWord.tabuWords = this.tabuWordsString
            .split(',')
            .map(s => s.trim())
            .filter(s => s !== '');

        if (this.currentWord.id) {
            this.adminService.updateWord(this.currentWord as any).subscribe({
                next: () => {
                    this.toastService.success('Kelime güncellendi.');
                    this.loadWords();
                    this.closeModal();
                },
                error: () => this.toastService.error('Güncelleme başarısız.')
            });
        } else {
            this.adminService.addWord(this.currentWord as any).subscribe({
                next: () => {
                    this.toastService.success('Yeni kelime eklendi.');
                    this.loadWords();
                    this.closeModal();
                },
                error: () => this.toastService.error('Ekleme başarısız.')
            });
        }
    }

    deleteWord(id: string): void {
        if (confirm('Bu kelimeyi silmek istediğinize emin misiniz?')) {
            this.adminService.deleteWord(id).subscribe({
                next: () => {
                    this.toastService.success('Kelime silindi.');
                    this.loadWords();
                },
                error: () => this.toastService.error('Silme işlemi başarısız.')
            });
        }
    }

    private getEmptyWord(): Word {
        return {
            id: '',
            targetWord: '',
            tabuWords: [],
            category: '',
            difficulty: 1
        };
    }
}
