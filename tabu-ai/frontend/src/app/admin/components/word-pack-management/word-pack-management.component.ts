import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService, WordPackAdmin } from '../../services/admin.service';
import { ToastService } from '../../../services/toast.service';

@Component({
    selector: 'app-word-pack-management',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './word-pack-management.component.html',
    styleUrls: ['./word-pack-management.component.css']
})
export class WordPackManagementComponent implements OnInit {
    wordPacks: WordPackAdmin[] = [];
    isLoading = false;
    isModalOpen = false;
    isEditing = false;

    currentPack = this.getEmptyPack();

    constructor(
        private adminService: AdminService,
        private toastService: ToastService
    ) { }

    ngOnInit(): void {
        this.loadWordPacks();
    }

    loadWordPacks(): void {
        this.isLoading = true;
        this.adminService.getAllWordPacks().subscribe({
            next: (packs) => {
                this.wordPacks = packs;
                this.isLoading = false;
            },
            error: () => {
                this.toastService.error('Kelime paketleri yüklenirken hata oluştu.');
                this.isLoading = false;
            }
        });
    }

    openModal(pack?: WordPackAdmin): void {
        if (pack) {
            this.isEditing = true;
            this.currentPack = {
                id: pack.id,
                name: pack.name,
                description: pack.description,
                language: pack.language,
                isPublic: pack.isPublic,
                isApproved: pack.isApproved
            };
        } else {
            this.isEditing = false;
            this.currentPack = this.getEmptyPack();
        }
        this.isModalOpen = true;
    }

    closeModal(): void {
        this.isModalOpen = false;
    }

    savePack(): void {
        if (this.isEditing) {
            this.adminService.updateWordPack(this.currentPack as any).subscribe({
                next: () => {
                    this.toastService.success('Kelime paketi güncellendi.');
                    this.loadWordPacks();
                    this.closeModal();
                },
                error: () => this.toastService.error('Güncelleme başarısız.')
            });
        } else {
            this.adminService.createWordPack(this.currentPack).subscribe({
                next: () => {
                    this.toastService.success('Yeni kelime paketi oluşturuldu.');
                    this.loadWordPacks();
                    this.closeModal();
                },
                error: () => this.toastService.error('Oluşturma başarısız.')
            });
        }
    }

    deletePack(id: string): void {
        if (confirm('Bu kelime paketini silmek istediğinize emin misiniz?')) {
            this.adminService.deleteWordPack(id).subscribe({
                next: () => {
                    this.toastService.success('Kelime paketi silindi.');
                    this.loadWordPacks();
                },
                error: () => this.toastService.error('Silme işlemi başarısız.')
            });
        }
    }

    private getEmptyPack() {
        return {
            id: '',
            name: '',
            description: '',
            language: 'tr',
            isPublic: true,
            isApproved: true
        };
    }
}
