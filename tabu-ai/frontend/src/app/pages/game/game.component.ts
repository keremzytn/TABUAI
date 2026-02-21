import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription, interval } from 'rxjs';
import { GameService } from '../../services/game.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { GameSession, GameResult } from '../../models/game.models';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container game-container">
      
      <!-- LOADING OVERLAY -->
      <div *ngIf="loading" class="loading-overlay fade-in">
        <div class="spinner-container">
          <div class="loading-ring">
            <div></div><div></div><div></div><div></div>
          </div>
          <p class="loading-text">{{ loadingText }}</p>
        </div>
      </div>

      <!-- GAME START SCREEN -->
      <div *ngIf="gameState === 'idle' && !loading" class="game-start fade-in">
        <div class="glass-card start-panel">
          <div class="start-header">
            <div class="start-icon pop-in">🎯</div>
            <h1 class="text-center">Mod Seç</h1>
            <p class="text-center text-muted">Nasıl oynamak istersin?</p>
          </div>
          
          <div class="mode-grid">
            <button class="mode-card glass-card" 
              [class.active]="selectedMode === 'demo'"
              (click)="selectedMode = 'demo'">
              <div class="mode-icon-wrapper">
                <div class="mode-icon">🎮</div>
              </div>
              <h3>Demo Modu</h3>
              <p>Hızlı başlangıç, kayıt gerektirmez.</p>
              <div class="mode-badge free">Ücretsiz</div>
            </button>

            <button class="mode-card glass-card" 
              [class.active]="selectedMode === 'real'"
              (click)="selectedMode = 'real'">
              <div class="mode-icon-wrapper">
                <div class="mode-icon">🤖</div>
              </div>
              <h3>AI Modu</h3>
              <p>Gerçek AI değerlendirmesi.</p>
              <div class="mode-badge ai">Groq AI</div>
            </button>
          </div>

          <!-- Category Selection -->
          <div class="category-section" *ngIf="selectedMode">
            <h4 class="section-label">Kategori (Opsiyonel)</h4>
            <div class="category-chips">
              <button *ngFor="let cat of categories" 
                class="chip"
                [class.active]="selectedCategory === cat"
                (click)="selectedCategory = selectedCategory === cat ? null : cat">
                {{ cat }}
              </button>
            </div>
          </div>

          <div class="actions mt-4 text-center">
            <button class="btn btn-primary btn-lg start-btn" (click)="startGame()">
              <span class="btn-text">OYUNA BAŞLA</span>
              <span class="btn-arrow">→</span>
            </button>
          </div>
        </div>
      </div>

      <!-- GAME PLAYING SCREEN -->
      <div *ngIf="gameState === 'playing' && currentGame" class="game-board fade-in">
        
        <!-- Header / Stats -->
        <div class="game-header glass-card">
          <div class="stat-item">
            <span class="label">SKOR</span>
            <span class="value score-glow" [class.score-bump]="scoreBump">{{ currentGame.score }}</span>
          </div>
          <div class="stat-item timer-item">
            <div class="timer-ring" [class.timer-warning]="isTimerWarning">{{ gameTime }}</div>
          </div>
          <div class="stat-item">
            <span class="label">DENEME</span>
            <span class="value">
              <span *ngFor="let a of [1,2,3]; let i = index" class="attempt-dot" [class.used]="i < currentGame.attemptNumber"></span>
            </span>
          </div>
        </div>

        <!-- Main Card Area -->
        <div class="card-area">
          <div class="target-card glass-card" [class.card-shake]="cardShake">
            <div class="card-header">
              <span class="difficulty-badge" [attr.data-level]="currentGame.word.difficulty">
                {{ getDifficultyText(currentGame.word.difficulty) }}
              </span>
              <span class="category-badge">{{ currentGame.word.category }}</span>
            </div>
            
            <div class="word-display">
              <h2 class="target-word slide-up">{{ currentGame.word.targetWord }}</h2>
            </div>

            <div class="tabu-section">
              <div class="tabu-label">
                <span class="tabu-icon">🚫</span> YASAKLI KELİMELER
              </div>
              <div class="tabu-list">
                <div *ngFor="let tabu of currentGame.word.tabuWords; let i = index" 
                     class="tabu-item" 
                     [style.animation-delay]="(i * 0.08) + 's'">
                  {{ tabu }}
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- History/Chat Section -->
        <div *ngIf="currentGame.attempts && currentGame.attempts.length > 0" class="history-area fade-in" #historyScroll>
          <div *ngFor="let attempt of currentGame.attempts" class="history-item glass-card" [class.correct]="attempt.isCorrect">
            <div class="history-prompt">
              <span class="badge">#{{ attempt.attemptNumber }}</span>
              <p>{{ attempt.userPrompt }}</p>
            </div>
            <div class="history-ai">
              <span class="ai-label">AI Tahmini:</span>
              <span class="ai-guess">"{{ attempt.aiGuess }}"</span>
              <span *ngIf="attempt.isCorrect" class="success-icon">✅</span>
              <span *ngIf="!attempt.isCorrect" class="fail-icon">❌</span>
            </div>
            <p class="ai-feedback">{{ attempt.aiFeedback }}</p>
          </div>
        </div>

        <!-- Input Area -->
        <div class="input-area glass-card" [class.input-error]="inputError" *ngIf="!submitting && !lastResult">
          <textarea 
            #promptInput
            [(ngModel)]="currentPrompt" 
            class="game-input" 
            placeholder="Yasaklı kelimeler kullanmadan hedefini anlat..."
            [disabled]="submitting"
            (keydown.enter)="onEnter($event)"
            (input)="checkTabuWords()"
            rows="1"></textarea>
          
          <button class="btn btn-send" (click)="submitPrompt()" [disabled]="!currentPrompt.trim() || submitting || currentGame.attemptNumber >= 3">
            <span *ngIf="!submitting" class="send-icon">➤</span>
            <div *ngIf="submitting" class="loading-spinner small"></div>
          </button>
        </div>

        <!-- Tabu Warning -->
        <div *ngIf="detectedTabuWord" class="tabu-warning slide-up">
          ⚠️ "<strong>{{ detectedTabuWord }}</strong>" yasaklı kelime!
        </div>
      </div>

      <!-- GAME RESULT MODAL -->
      <div *ngIf="lastResult" class="result-modal-backdrop fade-in" (click)="onBackdropClick($event)">
        <div *ngIf="lastResult.isCorrect" class="confetti-wrapper">
          <div *ngFor="let c of confettiPieces" class="confetti" [style]="c"></div>
        </div>

        <div class="result-modal minimal glass-card pop-in" [class.success]="lastResult.isCorrect" [class.fail]="!lastResult.isCorrect">
          
          <div class="modal-mini-header">
            <div class="header-main">
              <span class="mini-icon">{{ lastResult.isCorrect ? '🎉' : (currentGame?.attemptNumber === 3 ? '💀' : '😔') }}</span>
              <div class="title-group">
                <h2 class="mini-title">{{ lastResult.isCorrect ? 'BAŞARILI!' : (currentGame?.attemptNumber === 3 ? 'OYUN BİTTİ' : 'TEKRAR DENE') }}</h2>
                <p class="mini-msg">{{ lastResult.isCorrect ? 'AI bildi!' : 'AI bilemedi.' }}</p>
              </div>
            </div>
            
            <div class="mini-stats">
              <div class="mini-stat-item">
                <span class="ms-label">TAHMİN</span>
                <span class="ms-value" [class.correct]="lastResult.isCorrect">"{{ lastResult.aiGuess }}"</span>
              </div>
              <div *ngIf="lastResult.score > 0" class="mini-stat-item">
                <span class="ms-label">PUAN</span>
                <span class="ms-value score">+{{ lastResult.score }}</span>
              </div>
            </div>
          </div>

          <div class="mini-content">
            <!-- Quality & Feedback combined -->
            <div class="feedback-compact">
              <div class="quality-mini">
                <span *ngFor="let s of [1,2,3,4,5]" class="mini-star" [class.active]="s <= lastResult.promptQuality">★</span>
              </div>
              <p class="mini-feedback-text">"{{ lastResult.aiFeedback }}"</p>
            </div>

            <!-- Compact Suggestions (only if failed) -->
            <div *ngIf="!lastResult.isCorrect && lastResult.suggestions.length > 0" class="mini-suggestions">
              <span class="sugg-tag">İPUCU:</span>
              <span class="sugg-text">{{ lastResult.suggestions[0] }}</span>
            </div>
          </div>

          <div class="mini-actions">
            <button *ngIf="!lastResult.gameCompleted && (currentGame?.attemptNumber ?? 0) < 3" 
              class="btn-mini btn-primary" (click)="continueGame()">
              🔄 Tekrar Dene
            </button>
            <button class="btn-mini btn-secondary" (click)="startNewGame()">
              {{ lastResult.gameCompleted ? '🎮 Yeni Oyun' : '🚪 Çık' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .game-container {
      padding-bottom: 40px;
      min-height: 80vh;
      display: flex;
      flex-direction: column;
      justify-content: flex-start;
    }

    /* ===== Loading Overlay ===== */
    .loading-overlay {
      position: fixed;
      top: 0; left: 0; right: 0; bottom: 0;
      background: rgba(15, 23, 42, 0.92);
      z-index: 2000;
      display: flex;
      align-items: center;
      justify-content: center;
      backdrop-filter: blur(8px);
    }
    .spinner-container { display: flex; flex-direction: column; align-items: center; gap: 24px; }
    .loading-ring { display: inline-block; position: relative; width: 64px; height: 64px; }
    .loading-ring div { box-sizing: border-box; display: block; position: absolute; width: 52px; height: 52px; margin: 6px; border: 4px solid transparent; border-radius: 50%; animation: ring 1.2s cubic-bezier(0.5, 0, 0.5, 1) infinite; border-color: #8b5cf6 transparent transparent transparent; }
    .loading-ring div:nth-child(1) { animation-delay: -0.45s; }
    .loading-ring div:nth-child(2) { animation-delay: -0.3s; }
    .loading-ring div:nth-child(3) { animation-delay: -0.15s; }
    @keyframes ring { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
    .loading-text { color: #c4b5fd; font-size: 1.1rem; font-weight: 600; letter-spacing: 1px; animation: pulse-text 1.5s ease-in-out infinite; }
    @keyframes pulse-text { 0%, 100% { opacity: 0.6; } 50% { opacity: 1; } }

    /* ===== Start Screen ===== */
    .start-panel { padding: 40px; max-width: 650px; margin: 40px auto; }
    .start-header { text-align: center; margin-bottom: 32px; }
    .start-icon { font-size: 3.5rem; margin-bottom: 16px; }
    .start-header h1 { font-size: 1.8rem; font-weight: 700; margin-bottom: 4px; }
    .mode-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
    .mode-card { background: rgba(255,255,255,0.03); border: 2px solid rgba(255,255,255,0.08); padding: 28px 20px; border-radius: 20px; cursor: pointer; text-align: center; transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1); position: relative; overflow: hidden; }
    .mode-card.active { border-color: var(--primary); transform: scale(1.02); box-shadow: 0 0 30px rgba(139, 92, 246, 0.15); }
    .mode-icon { font-size: 2.8rem; transition: transform 0.3s; }
    .mode-card:hover .mode-icon { transform: scale(1.1) rotate(-5deg); }
    .mode-card h3 { font-size: 1.1rem; font-weight: 700; margin-bottom: 6px; }
    .mode-card p { font-size: 0.85rem; color: var(--text-muted); line-height: 1.4; }
    .mode-badge { display: inline-block; margin-top: 12px; padding: 3px 12px; border-radius: 20px; font-size: 0.75rem; font-weight: 700; }
    .mode-badge.free { background: rgba(16, 185, 129, 0.2); color: #6ee7b7; }
    .mode-badge.ai { background: rgba(139, 92, 246, 0.2); color: #c4b5fd; }
    .category-chips { display: flex; flex-wrap: wrap; gap: 8px; }
    .chip { background: rgba(255, 255, 255, 0.05); border: 1px solid rgba(255, 255, 255, 0.1); color: var(--text-muted); padding: 6px 16px; border-radius: 20px; font-size: 0.85rem; cursor: pointer; transition: all 0.2s; }
    .chip.active { background: rgba(139, 92, 246, 0.2); border-color: var(--primary); color: #c4b5fd; }

    /* ===== Game Header ===== */
    .game-header { display: flex; justify-content: space-between; align-items: center; padding: 16px 24px; margin-bottom: 24px; }
    .stat-item { display: flex; flex-direction: column; align-items: center; gap: 4px; }
    .stat-item .label { font-size: 0.7rem; color: var(--text-muted); letter-spacing: 1.5px; font-weight: 600; }
    .stat-item .value { font-size: 1.25rem; font-weight: 700; font-family: 'JetBrains Mono', monospace; }
    .attempt-dot { display: inline-block; width: 10px; height: 10px; border-radius: 50%; background: rgba(255, 255, 255, 0.15); margin: 0 3px; }
    .attempt-dot.used { background: var(--primary); box-shadow: 0 0 6px var(--primary); }
    .timer-ring { font-size: 1.4rem; font-weight: 700; color: white; background: rgba(255,255,255,0.08); padding: 6px 20px; border-radius: 24px; font-family: 'JetBrains Mono', monospace; }
    .timer-warning { color: #fbbf24; border-color: rgba(251, 191, 36, 0.3); animation: timer-pulse 1s ease infinite; }

    /* ===== Card Area ===== */
    .card-area { display: flex; justify-content: center; margin-bottom: 32px; }
    .target-card { width: 100%; max-width: 420px; background: linear-gradient(145deg, rgba(30, 41, 59, 0.95), rgba(15, 23, 42, 0.98)); border: 1px solid rgba(255, 255, 255, 0.12); border-radius: 24px; overflow: hidden; }
    .card-header { display: flex; justify-content: space-between; padding: 14px 18px; background: rgba(0,0,0,0.25); }
    .target-word { font-size: 2.5rem; text-align: center; color: white; text-transform: uppercase; margin: 30px 0; }
    .tabu-section { padding: 20px; background: rgba(239, 68, 68, 0.04); }
    .tabu-list { display: grid; grid-template-columns: 1fr 1fr; gap: 8px; }
    .tabu-item { background: rgba(239, 68, 68, 0.08); color: #fca5a5; padding: 8px; text-align: center; border-radius: 8px; font-size: 0.9rem; }

    /* ===== History Area ===== */
    .history-area { max-width: 500px; margin: 0 auto 24px; display: flex; flex-direction: column; gap: 16px; overflow-y: auto; max-height: 300px; padding-right: 8px; }
    .history-item { padding: 16px; border-radius: 16px; border-left: 4px solid var(--primary); position: relative; }
    .history-item.correct { border-left-color: #10b981; background: rgba(16, 185, 129, 0.05); }
    .history-prompt { display: flex; gap: 10px; margin-bottom: 8px; }
    .history-prompt .badge { background: var(--primary); font-size: 0.7rem; height: fit-content; padding: 2px 6px; border-radius: 4px; }
    .history-prompt p { margin: 0; font-size: 0.95rem; line-height: 1.4; color: #e2e8f0; }
    .history-ai { background: rgba(255,255,255,0.03); padding: 8px 12px; border-radius: 8px; display: flex; align-items: center; gap: 8px; font-size: 0.9rem; margin-bottom: 8px; }
    .ai-label { color: var(--text-muted); font-weight: 600; }
    .ai-guess { color: #c4b5fd; font-weight: 700; font-style: italic; }
    .ai-feedback { font-size: 0.85rem; color: var(--text-muted); margin: 0; }

    /* ===== Minimal Result Modal ===== */
    .result-modal-backdrop {
      position: fixed;
      top: 0; left: 0; right: 0; bottom: 0;
      background: rgba(15, 23, 42, 0.4); /* Much more transparent */
      z-index: 2500;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 20px;
      backdrop-filter: blur(4px); /* Reduced blur */
    }

    .result-modal.minimal {
      max-width: 440px;
      width: 95%;
      padding: 0;
      border-radius: 24px;
      background: rgba(30, 41, 59, 0.95);
      border: 1px solid rgba(255, 255, 255, 0.15);
      box-shadow: 0 20px 40px -10px rgba(0, 0, 0, 0.6);
      transform-origin: center;
    }

    .modal-mini-header {
      padding: 24px;
      background: rgba(255,255,255,0.03);
      border-bottom: 1px solid rgba(255,255,255,0.05);
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .header-main {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .mini-icon { font-size: 2.5rem; }

    .title-group { flex: 1; }
    .mini-title { font-size: 1.25rem; font-weight: 800; margin: 0; letter-spacing: 1px; }
    .mini-msg { font-size: 0.85rem; color: var(--text-muted); margin: 0; }

    .mini-stats {
      display: flex;
      gap: 12px;
    }

    .mini-stat-item {
      flex: 1;
      background: rgba(0,0,0,0.2);
      padding: 10px 14px;
      border-radius: 12px;
      display: flex;
      flex-direction: column;
      align-items: center;
    }

    .ms-label { font-size: 0.6rem; color: var(--text-muted); font-weight: 700; margin-bottom: 2px; }
    .ms-value { font-size: 1.1rem; font-weight: 800; }
    .ms-value.correct { color: var(--accent); }
    .ms-value.score { color: #fbbf24; }

    .mini-content {
      padding: 20px 24px;
    }

    .feedback-compact {
      margin-bottom: 16px;
      text-align: center;
    }

    .quality-mini {
      display: flex;
      justify-content: center;
      gap: 4px;
      margin-bottom: 8px;
    }
    .mini-star { font-size: 1.1rem; color: rgba(255,255,255,0.1); }
    .mini-star.active { color: #eab308; }

    .mini-feedback-text {
      font-size: 0.85rem;
      color: #94a3b8;
      font-style: italic;
      line-height: 1.4;
      margin: 0;
    }

    .mini-suggestions {
      background: rgba(139, 92, 246, 0.1);
      padding: 10px 14px;
      border-radius: 10px;
      border-left: 3px solid var(--primary);
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 0.8rem;
    }
    .sugg-tag { font-weight: 800; color: var(--primary); flex-shrink: 0; }
    .sugg-text { color: #cbd5e1; }

    .mini-actions {
      padding: 16px 24px 24px;
      display: flex;
      gap: 12px;
    }

    .btn-mini {
      flex: 1;
      padding: 12px;
      border-radius: 12px;
      border: none;
      font-weight: 700;
      font-size: 0.9rem;
      cursor: pointer;
      transition: all 0.2s;
    }
    .btn-mini.btn-primary { 
      background: var(--primary); color: #fff;
      box-shadow: 0 4px 12px rgba(139, 92, 246, 0.3);
    }
    .btn-mini.btn-secondary { 
      background: rgba(255,255,255,0.05); color: #fff;
      border: 1px solid rgba(255,255,255,0.1);
    }

    /* ===== Input Area ===== */
    .input-area { display: flex; gap: 12px; padding: 14px; align-items: flex-end; max-width: 500px; margin: 0 auto; width: 100%; }
    .game-input { flex: 1; background: transparent; border: none; color: white; resize: none; font-size: 1rem; min-height: 48px; max-height: 120px; padding: 12px; }
    .game-input:focus { outline: none; }
    .btn-send { background: linear-gradient(135deg, var(--primary), #7c3aed); color: white; width: 52px; height: 52px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 1.2rem; }
    .btn-send:disabled { background: #475569; }

    .bounce { animation: bounce 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275); }
    @keyframes bounce { 0% { transform: scale(0.3); opacity: 0; } 50% { transform: scale(1.1); } 70% { transform: scale(0.9); } 100% { transform: scale(1); opacity: 1; } }

    .confetti-wrapper { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; pointer-events: none; z-index: 2600; }
    .confetti { position: absolute; top: -10px; border-radius: 2px; animation: confetti-fall 3s linear infinite; }
    @keyframes confetti-fall { 
      0% { transform: translateY(0) rotate(0); opacity: 1; } 
      100% { transform: translateY(100vh) rotate(720deg); opacity: 0; } 
    }

    @media (max-width: 768px) {
      .mode-grid { grid-template-columns: 1fr; }
      .modal-buttons { flex-direction: column; }
    }
  `]
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
    private toastService: ToastService
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