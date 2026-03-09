import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { WordPackService } from '../../services/wordpack.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { WordPack, CreateWordPackRequest, CreateWordInPackRequest, GameLanguage, SUPPORTED_LANGUAGES } from '../../models/game.models';

@Component({
  selector: 'app-word-packs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './word-packs.component.html',
  styleUrl: './word-packs.component.scss'
})
export class WordPacksComponent implements OnInit {
  activeTab: 'browse' | 'my' | 'create' = 'browse';
  selectedLanguage: GameLanguage = 'tr';
  languages = SUPPORTED_LANGUAGES;

  publicPacks: WordPack[] = [];
  myPacks: WordPack[] = [];
  loading = false;

  // Create form
  newPack: CreateWordPackRequest = {
    name: '',
    description: '',
    language: 'tr',
    isPublic: true,
    words: []
  };
  newWord: CreateWordInPackRequest = {
    targetWord: '',
    tabuWords: ['', '', '', '', ''],
    category: '',
    difficulty: 1
  };
  creating = false;

  constructor(
    private wordPackService: WordPackService,
    private authService: AuthService,
    private toastService: ToastService,
    public router: Router
  ) {}

  ngOnInit() {
    this.loadPublicPacks();
  }

  loadPublicPacks() {
    this.loading = true;
    this.wordPackService.getPublicPacks(this.selectedLanguage).subscribe({
      next: (packs) => { this.publicPacks = packs; this.loading = false; },
      error: () => { this.loading = false; this.toastService.error('Paketler yüklenemedi.'); }
    });
  }

  loadMyPacks() {
    const user = this.authService.currentUserValue;
    if (!user) return;
    this.loading = true;
    this.wordPackService.getMyPacks(user.id).subscribe({
      next: (packs) => { this.myPacks = packs; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  switchTab(tab: 'browse' | 'my' | 'create') {
    this.activeTab = tab;
    if (tab === 'browse') this.loadPublicPacks();
    if (tab === 'my') this.loadMyPacks();
  }

  likePack(pack: WordPack) {
    this.wordPackService.likePack(pack.id).subscribe({
      next: (res) => { pack.likeCount = res.likeCount; },
      error: () => this.toastService.error('Beğeni eklenemedi.')
    });
  }

  deletePack(pack: WordPack) {
    const user = this.authService.currentUserValue;
    if (!user) return;
    this.wordPackService.deletePack(pack.id, user.id).subscribe({
      next: () => {
        this.myPacks = this.myPacks.filter(p => p.id !== pack.id);
        this.toastService.success('Paket silindi.');
      },
      error: () => this.toastService.error('Paket silinemedi.')
    });
  }

  addWordToPack() {
    if (!this.newWord.targetWord.trim()) return;
    const tabuWords = this.newWord.tabuWords.filter(t => t.trim());
    if (tabuWords.length < 3) {
      this.toastService.warning('En az 3 tabu kelime gerekli.');
      return;
    }

    this.newPack.words.push({
      targetWord: this.newWord.targetWord.trim(),
      tabuWords: tabuWords.map(t => t.trim()),
      category: this.newWord.category || 'Genel',
      difficulty: this.newWord.difficulty
    });

    this.newWord = { targetWord: '', tabuWords: ['', '', '', '', ''], category: '', difficulty: 1 };
  }

  removeWordFromPack(index: number) {
    this.newPack.words.splice(index, 1);
  }

  createPack() {
    const user = this.authService.currentUserValue;
    if (!user) { this.toastService.warning('Giriş yapmalısınız.'); return; }
    if (!this.newPack.name.trim()) { this.toastService.warning('Paket adı gerekli.'); return; }
    if (this.newPack.words.length < 3) { this.toastService.warning('En az 3 kelime ekleyin.'); return; }

    this.creating = true;
    this.newPack.language = this.selectedLanguage;

    this.wordPackService.createPack(this.newPack, user.id).subscribe({
      next: () => {
        this.toastService.success('Paket oluşturuldu! Onay bekleniyor.');
        this.newPack = { name: '', description: '', language: this.selectedLanguage, isPublic: true, words: [] };
        this.creating = false;
        this.switchTab('my');
      },
      error: () => {
        this.creating = false;
        this.toastService.error('Paket oluşturulamadı.');
      }
    });
  }
}
