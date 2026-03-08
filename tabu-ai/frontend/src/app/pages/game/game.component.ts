import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription, interval } from 'rxjs';
import { GameService } from '../../services/game.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { GameSession, GameResult } from '../../models/game.models';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss'
})
export class GameComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('promptInput') promptInput!: ElementRef;
  @ViewChild('historyScroll') historyScroll!: ElementRef;

  gameState: 'idle' | 'playing' | 'completed' = 'idle';
  currentGame: GameSession | null = null;
  selectedMode: 'demo' | 'real' = 'demo';
  selectedCategory: string | null = null;
  currentPrompt = '';
  lastResult: GameResult | null = null;

  loading = false;
  loadingText = 'Oyun Hazırlanıyor...';
  submitting = false;
  gameTime = '00:00';
  isTimerWarning = false;
  scoreBump = false;
  cardShake = false;
  inputError = false;
  detectedTabuWord: string | null = null;

  categories = ['Ulaşım', 'Teknoloji', 'Bilim', 'Sanat', 'Yemek', 'Spor', 'Tarih', 'Doğa', 'Müzik'];
  confettiPieces: string[] = [];

  private gameTimer: Subscription | null = null;
  private startTime: Date | null = null;

  constructor(
    private gameService: GameService,
    private authService: AuthService,
    private toastService: ToastService,
    private router: Router
  ) { }

  ngOnInit() {
    this.gameService.gameState$.subscribe(state => {
      this.gameState = state;
      if (state === 'playing') {
        this.startGameTimer();
      } else {
        this.stopGameTimer();
      }
    });

    this.gameService.currentGame$.subscribe(game => {
      this.currentGame = game;
    });
  }

  ngOnDestroy() {
    this.stopGameTimer();
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    if (this.historyScroll) {
      const element = this.historyScroll.nativeElement;
      element.scrollTop = element.scrollHeight;
    }
  }

  startGame() {
    this.loading = true;
    this.loadingText = this.selectedMode === 'demo' ? 'Demo Oyunu Hazırlanıyor...' : 'AI ile Bağlantı Kuruluyor...';
    this.lastResult = null;

    const user = this.authService.currentUserValue;
    const userId = user?.id ?? 'demo-user';

    const request = {
      userId: userId,
      gameMode: 'Solo',
      category: this.selectedCategory ?? undefined,
      difficulty: undefined
    };

    const gameObservable = this.selectedMode === 'demo'
      ? this.gameService.startDemoGame()
      : this.gameService.startGame(request);

    gameObservable.subscribe({
      next: (game) => {
        this.gameService.setCurrentGame(game);
        this.loading = false;
        this.toastService.info(`"${game.word.targetWord}" kelimesini anlat!`);
        window.scrollTo(0, 0);
      },
      error: (error) => {
        console.error('Game start error:', error);
        this.loading = false;
        this.toastService.error('Oyun başlatılamadı. Lütfen tekrar deneyin.');
      }
    });
  }

  onEnter(event: Event) {
    if (!event) return;
    const keyboardEvent = event as KeyboardEvent;
    if (!keyboardEvent.shiftKey) {
      event.preventDefault();
      this.submitPrompt();
    }
  }

  checkTabuWords() {
    if (!this.currentGame) return;
    this.detectedTabuWord = null;

    // Normalize prompt: replace punctuation with space, convert to lower case and split by whitespace
    const wordsInPrompt = this.currentPrompt.toLowerCase()
      .replace(/[.,!?;:()"]/g, ' ')
      .split(/\s+/)
      .filter(w => w.length > 0);

    for (const tabu of this.currentGame.word.tabuWords) {
      const tabuLower = tabu.toLowerCase();

      for (const word of wordsInPrompt) {
        // 1. Exact match - Always forbidden
        if (word === tabuLower) {
          this.detectedTabuWord = tabu;
          return;
        }

        // 2. Starts with check - Needs careful logic for Turkish
        if (word.startsWith(tabuLower)) {
          const suffix = word.substring(tabuLower.length);

          /* 
            GENERAL TURKISH HEURISTIC:
            If the forbidden word is very short (<=3 chars like 'at', 'ay', 'su', 'top'), 
            we ONLY block it if the suffix is a standard inflectional suffix (çekim eki).
            Unrelated roots like 'atlet', 'ayna', 'susam', 'toplam' will be ALLOWED.
          */

          // Regular Turkish Inflectional Suffixes (Çekim Ekleri)
          // Includes plural (-lar), possessive (-ım, -ın), cases (-da, -dan, -a), etc.
          const turkishInflectionRegex = /^(lar|ler|ı|i|u|ü|ın|in|un|ün|ım|im|um|üm|ız|iz|uz|üz|da|de|ta|te|dan|den|tan|ten|y|s|n|a|e)*$/;

          if (tabuLower.length <= 3) {
            // Very strict for short roots
            // 'top-lam' -> 'lam' is NOT in turkishInflectionRegex (mostly)
            // We also specifically exclude common root-extending syllables
            const rootExtenders = ['lam', 'lum', 'rak', 'na', 'let', 'lan', 'len', 'sam', 'kin'];

            if (turkishInflectionRegex.test(suffix) && !rootExtenders.some(ext => suffix.startsWith(ext))) {
              this.detectedTabuWord = tabu;
              return;
            }
          } else {
            // For longer roots (e.g. 'bilgisayar'), if the word starts with it, 
            // it's almost certainly the word itself or a direct derivative.
            // But we still apply a loose check to be safe.
            if (turkishInflectionRegex.test(suffix) || suffix.length <= 3) {
              this.detectedTabuWord = tabu;
              return;
            }
          }
        }
      }
    }
  }

  submitPrompt() {
    if (!this.currentGame || !this.currentPrompt.trim() || this.submitting) return;

    if (this.detectedTabuWord) {
      this.inputError = true;
      this.cardShake = true;
      this.toastService.warning(`"${this.detectedTabuWord}" yasaklı bir kelime! Kullanmamalısın.`);
      setTimeout(() => { this.inputError = false; this.cardShake = false; }, 500);
      return;
    }

    this.submitting = true;
    const promptToSend = this.currentPrompt;
    this.currentPrompt = ''; // Clear early for UX

    const submitObservable = this.selectedMode === 'demo'
      ? this.gameService.submitDemoPrompt(
        promptToSend,
        this.currentGame.word.targetWord,
        this.currentGame.word.tabuWords
      )
      : this.gameService.submitPrompt({
        gameSessionId: this.currentGame.id,
        prompt: promptToSend
      });

    submitObservable.subscribe({
      next: (result) => {
        this.lastResult = result;
        this.submitting = false;

        if (this.currentGame) {
          // Update game session
          this.currentGame.score += result.score;
          this.currentGame.attemptNumber = result.history?.length || (this.currentGame.attemptNumber + 1);
          this.currentGame.attempts = result.history || [
            ...(this.currentGame.attempts || []),
            {
              attemptNumber: this.currentGame.attemptNumber,
              userPrompt: promptToSend,
              aiGuess: result.aiGuess,
              isCorrect: result.isCorrect,
              score: result.score,
              aiFeedback: result.aiFeedback,
              promptQuality: result.promptQuality,
              createdAt: new Date().toISOString()
            }
          ];

          if (result.score > 0) {
            this.scoreBump = true;
            setTimeout(() => this.scoreBump = false, 400);
          }

          if (result.gameCompleted) {
            this.gameService.completeGame();
            if (result.isCorrect) {
              this.toastService.success(`Tebrikler! +${result.score} puan kazandın! 🎉`);
              this.generateConfetti();
            } else {
              this.toastService.warning('Maalesef tüm hakların bitti.');
            }
          } else {
            if (result.isCorrect) {
              this.toastService.success('AI doğru tahmin etti!');
            } else {
              this.toastService.info('AI tahmin edemedi. Tekrar dene!');
            }
          }
        }
      },
      error: (error) => {
        console.error('Submit prompt error:', error);
        this.submitting = false;
        this.currentPrompt = promptToSend; // Restore prompt on error
        this.toastService.error('Prompt gönderilemedi. Lütfen tekrar deneyin.');
      }
    });
  }

  continueGame() {
    this.lastResult = null;
    this.currentPrompt = '';
    this.detectedTabuWord = null;
    setTimeout(() => this.promptInput?.nativeElement?.focus(), 100);
  }

  startNewGame() {
    this.gameState = 'idle';
    this.currentGame = null;
    this.lastResult = null;
    this.currentPrompt = '';
    this.detectedTabuWord = null;
    this.gameService.setCurrentGame(null);
  }

  onBackdropClick(event: MouseEvent) {
    if ((event.target as HTMLElement).classList.contains('result-modal-backdrop')) {
      if (this.lastResult?.gameCompleted) {
        this.startNewGame();
      }
    }
  }

  goToVersus(): void {
    this.router.navigate(['/versus']);
  }

  getDifficultyText(level: number): string {
    const levels: Record<number, string> = { 1: 'KOLAY', 2: 'ORTA', 3: 'ZOR', 4: 'UZMAN' };
    return levels[level] || '???';
  }

  private generateConfetti() {
    const colors = ['#8b5cf6', '#10b981', '#eab308', '#ef4444', '#6366f1', '#f97316'];
    this.confettiPieces = Array.from({ length: 150 }, (_, i) => {
      const color = colors[i % colors.length];
      const left = Math.random() * 100;
      const delay = Math.random() * 3;
      const size = Math.random() * 8 + 6;
      const duration = Math.random() * 2 + 3;
      return `left: ${left}%; background: ${color}; animation-delay: ${delay}s; width: ${size}px; height: ${size}px; animation-duration: ${duration}s;`;
    });
  }

  private startGameTimer() {
    if (this.gameTimer) return;
    this.startTime = new Date();
    this.isTimerWarning = false;
    this.gameTimer = interval(1000).subscribe(() => this.updateGameTime());
  }

  private stopGameTimer() {
    if (this.gameTimer) {
      this.gameTimer.unsubscribe();
      this.gameTimer = null;
    }
    this.gameTime = '00:00';
  }

  private updateGameTime() {
    if (!this.startTime) return;
    const now = new Date();
    const elapsed = Math.floor((now.getTime() - this.startTime.getTime()) / 1000);
    const minutes = Math.floor(elapsed / 60);
    const seconds = elapsed % 60;
    this.gameTime = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;

    if (elapsed > 120) {
      this.isTimerWarning = true;
    }
  }
}