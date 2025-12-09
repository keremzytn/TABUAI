import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription, interval } from 'rxjs';
import { GameService } from '../../services/game.service';
import { GameSession, GameResult } from '../../models/game.models';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container">
      <!-- Game Start Screen -->
      <div *ngIf="gameState === 'idle'" class="game-start fade-in">
        <div class="start-card">
          <h1>🎯 Oyuna Başla</h1>
          <p>Prompt engineering becerilerinizi test etmeye hazır mısınız?</p>
          
          <div class="game-modes">
            <button 
              class="mode-btn"
              [class.selected]="selectedMode === 'demo'"
              (click)="selectedMode = 'demo'">
              <div class="mode-icon">🎮</div>
              <div class="mode-title">Demo Modu</div>
              <div class="mode-desc">Hemen başlayın</div>
            </button>
            <button 
              class="mode-btn"
              [class.selected]="selectedMode === 'real'"
              (click)="selectedMode = 'real'">
              <div class="mode-icon">🤖</div>
              <div class="mode-title">AI Modu</div>
              <div class="mode-desc">Gerçek AI değerlendirmesi</div>
            </button>
          </div>

          <button 
            class="btn btn-primary btn-large"
            (click)="startGame()"
            [disabled]="loading">
            <span *ngIf="loading" class="loading"></span>
            {{ loading ? 'Başlatılıyor...' : '🚀 Başla' }}
          </button>
        </div>
      </div>

      <!-- Game Playing Screen -->
      <div *ngIf="gameState === 'playing' && currentGame" class="game-board fade-in">
        <!-- Game Stats -->
        <div class="game-header">
          <div class="stats-row">
            <div class="stat">
              <span class="stat-label">Kategori</span>
              <span class="stat-value">{{ currentGame.word.category }}</span>
            </div>
            <div class="stat">
              <span class="stat-label">Zorluk</span>
              <span class="stat-value">{{ getDifficultyText(currentGame.word.difficulty) }}</span>
            </div>
            <div class="stat">
              <span class="stat-label">Deneme</span>
              <span class="stat-value">{{ currentGame.attemptNumber }}/3</span>
            </div>
            <div class="stat">
              <span class="stat-label">Süre</span>
              <span class="stat-value">{{ gameTime }}</span>
            </div>
          </div>
          <div class="score-display">
            💎 {{ currentGame.score }}
          </div>
        </div>

        <!-- Game Content -->
        <div class="game-content">
          <!-- Target Word Section -->
          <div class="word-section">
            <div class="target-card">
              <h3>🎯 Hedef Kelime</h3>
              <div class="target-word">{{ currentGame.word.targetWord }}</div>
            </div>

            <div class="tabu-card">
              <h4>🚫 Yasaklı Kelimeler</h4>
              <div class="tabu-words">
                <span 
                  *ngFor="let tabu of currentGame.word.tabuWords" 
                  class="tabu-word">
                  {{ tabu }}
                </span>
              </div>
            </div>
          </div>

          <!-- Prompt Section -->
          <div class="prompt-section">
            <h3>✍️ Promptunuzu Yazın</h3>
            
            <textarea
              [(ngModel)]="currentPrompt"
              class="prompt-input"
              placeholder="AI'nın hedef kelimeyi tahmin edebilmesi için açıklayıcı bir prompt yazın..."
              [disabled]="submitting"></textarea>

            <div class="prompt-actions">
              <button 
                class="btn btn-primary"
                (click)="submitPrompt()"
                [disabled]="!currentPrompt.trim() || submitting">
                <span *ngIf="submitting" class="loading"></span>
                {{ submitting ? 'AI Düşünüyor...' : '🤖 Gönder' }}
              </button>
              <button 
                class="btn btn-secondary"
                (click)="clearPrompt()"
                [disabled]="submitting">
                Temizle
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Game Result Screen -->
      <div *ngIf="lastResult" class="game-result fade-in">
        <div class="result-card" [class.success]="lastResult.isCorrect" [class.failure]="!lastResult.isCorrect">
          <div class="result-icon">{{ lastResult.isCorrect ? '🎉' : '😔' }}</div>
          <h2>{{ lastResult.isCorrect ? 'Tebrikler!' : 'Tekrar Deneyin' }}</h2>
          <p class="result-subtitle">{{ lastResult.isCorrect ? 'AI doğru tahmin etti!' : 'AI yanlış tahmin etti.' }}</p>

          <div class="result-details">
            <div class="detail">
              <span class="detail-label">AI Tahmini</span>
              <span class="detail-value">{{ lastResult.aiGuess }}</span>
            </div>
            <div class="detail">
              <span class="detail-label">Puan</span>
              <span class="detail-value score">+{{ lastResult.score }}</span>
            </div>
            <div class="detail">
              <span class="detail-label">Kalite</span>
              <span class="detail-value">{{ getQualityText(lastResult.promptQuality) }}</span>
            </div>
          </div>

          <div class="feedback-box">
            <h4>💬 Geri Bildirim</h4>
            <p>{{ lastResult.aiFeedback }}</p>
          </div>

          <div *ngIf="lastResult.suggestions.length > 0" class="suggestions-box">
            <h4>💡 Öneriler</h4>
            <ul>
              <li *ngFor="let suggestion of lastResult.suggestions">{{ suggestion }}</li>
            </ul>
          </div>

          <div class="result-actions">
            <button 
              *ngIf="!lastResult.gameCompleted && (currentGame?.attemptNumber ?? 0) < 3"
              class="btn btn-primary"
              (click)="continueGame()">
              🔄 Tekrar Dene
            </button>
            <button 
              class="btn btn-secondary"
              (click)="startNewGame()">
              🎮 Yeni Oyun
            </button>
            <button 
              class="btn btn-secondary"
              (click)="goHome()">
              🏠 Ana Sayfa
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    /* Start Screen */
    .game-start {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 60vh;
    }

    .start-card {
      background: white;
      border-radius: 12px;
      border: 1px solid #e2e8f0;
      padding: 40px;
      text-align: center;
      max-width: 600px;
      width: 100%;
    }

    .start-card h1 {
      font-size: 2rem;
      font-weight: 700;
      margin-bottom: 12px;
      color: #1a202c;
    }

    .start-card > p {
      color: #718096;
      margin-bottom: 32px;
    }

    .game-modes {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      margin-bottom: 32px;
    }

    .mode-btn {
      padding: 24px 16px;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      background: white;
      cursor: pointer;
      transition: all 0.2s ease;
    }

    .mode-btn.selected {
      border-color: #667eea;
      background: #f7fafc;
    }

    .mode-btn:hover {
      border-color: #cbd5e0;
    }

    .mode-icon {
      font-size: 2rem;
      margin-bottom: 8px;
    }

    .mode-title {
      font-weight: 600;
      color: #2d3748;
      margin-bottom: 4px;
    }

    .mode-desc {
      font-size: 0.875rem;
      color: #718096;
    }

    .btn-large {
      padding: 14px 32px;
      font-size: 1rem;
    }

    /* Game Board */
    .game-board {
      max-width: 1100px;
      margin: 0 auto;
    }

    .game-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: white;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      padding: 24px;
      margin-bottom: 24px;
      gap: 24px;
    }

    .stats-row {
      display: flex;
      gap: 32px;
      flex-wrap: wrap;
    }

    .stat {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .stat-label {
      font-size: 0.875rem;
      color: #718096;
    }

    .stat-value {
      font-weight: 600;
      color: #2d3748;
    }

    .game-content {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 24px;
    }

    .word-section, .prompt-section {
      background: white;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      padding: 24px;
    }

    .target-card {
      text-align: center;
      margin-bottom: 24px;
    }

    .target-card h3 {
      margin-bottom: 16px;
      color: #2d3748;
      font-size: 1.125rem;
    }

    .target-word {
      font-size: 2.5rem;
      font-weight: 700;
      color: #667eea;
      background: #f7fafc;
      border-radius: 8px;
      padding: 20px;
    }

    .tabu-card h4 {
      margin-bottom: 16px;
      color: #2d3748;
      text-align: center;
      font-size: 1rem;
    }

    .tabu-words {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      justify-content: center;
    }

    .prompt-section h3 {
      margin-bottom: 16px;
      color: #2d3748;
      font-size: 1.125rem;
    }

    .prompt-input {
      width: 100%;
      min-height: 200px;
      margin-bottom: 16px;
    }

    .prompt-actions {
      display: flex;
      gap: 12px;
    }

    /* Result Screen */
    .game-result {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 60vh;
    }

    .result-card {
      background: white;
      border: 2px solid #e2e8f0;
      border-radius: 12px;
      padding: 40px;
      max-width: 600px;
      width: 100%;
      text-align: center;
    }

    .result-card.success {
      border-color: #48bb78;
    }

    .result-card.failure {
      border-color: #fc8181;
    }

    .result-icon {
      font-size: 4rem;
      margin-bottom: 16px;
    }

    .result-card h2 {
      margin-bottom: 8px;
      color: #1a202c;
      font-size: 1.75rem;
    }

    .result-subtitle {
      color: #718096;
      margin-bottom: 32px;
    }

    .result-details {
      display: grid;
      gap: 12px;
      margin-bottom: 24px;
      text-align: left;
    }

    .detail {
      display: flex;
      justify-content: space-between;
      padding: 12px 16px;
      background: #f7fafc;
      border-radius: 6px;
    }

    .detail-label {
      color: #718096;
      font-weight: 500;
    }

    .detail-value {
      font-weight: 600;
      color: #2d3748;
    }

    .score {
      color: #48bb78;
    }

    .feedback-box, .suggestions-box {
      text-align: left;
      margin-bottom: 24px;
      padding: 16px;
      background: #f7fafc;
      border-radius: 8px;
    }

    .feedback-box h4, .suggestions-box h4 {
      margin-bottom: 12px;
      color: #2d3748;
      font-size: 1rem;
    }

    .feedback-box p {
      color: #4a5568;
      line-height: 1.6;
    }

    .suggestions-box ul {
      margin: 0;
      padding-left: 20px;
    }

    .suggestions-box li {
      margin-bottom: 8px;
      color: #4a5568;
    }

    .result-actions {
      display: flex;
      gap: 12px;
      justify-content: center;
      flex-wrap: wrap;
    }

    /* Responsive */
    @media (max-width: 768px) {
      .game-modes {
        grid-template-columns: 1fr;
      }

      .game-header {
        flex-direction: column;
        align-items: stretch;
      }

      .stats-row {
        justify-content: space-between;
        gap: 16px;
      }

      .game-content {
        grid-template-columns: 1fr;
      }

      .result-actions {
        flex-direction: column;
      }

      .result-actions button {
        width: 100%;
      }
    }
  `]
})
export class GameComponent implements OnInit, OnDestroy {
  gameState: 'idle' | 'playing' | 'completed' = 'idle';
  currentGame: GameSession | null = null;
  selectedMode: 'demo' | 'real' = 'demo';
  currentPrompt = '';
  lastResult: GameResult | null = null;

  loading = false;
  submitting = false;
  gameTime = '00:00';

  private gameTimer: Subscription | null = null;
  private startTime: Date | null = null;

  constructor(private gameService: GameService) { }

  ngOnInit() {
    this.gameService.setCurrentGame(null);

    this.gameService.gameState$.subscribe(state => {
      this.gameState = state;
    });

    this.gameService.currentGame$.subscribe(game => {
      this.currentGame = game;
      if (game && this.gameState === 'playing') {
        this.startGameTimer();
      }
    });
  }

  ngOnDestroy() {
    this.stopGameTimer();
  }

  startGame() {
    this.loading = true;
    this.lastResult = null;

    const request = {
      userId: 'demo-user',
      gameMode: 'Solo',
      category: undefined,
      difficulty: undefined
    };

    const gameObservable = this.selectedMode === 'demo'
      ? this.gameService.startDemoGame()
      : this.gameService.startGame(request);

    gameObservable.subscribe({
      next: (game) => {
        this.gameService.setCurrentGame(game);
        this.loading = false;
      },
      error: (error) => {
        console.error('Game start error:', error);
        this.loading = false;
      }
    });
  }

  submitPrompt() {
    if (!this.currentGame || !this.currentPrompt.trim()) return;

    this.submitting = true;

    const submitObservable = this.selectedMode === 'demo'
      ? this.gameService.submitDemoPrompt(
        this.currentPrompt,
        this.currentGame.word.targetWord,
        this.currentGame.word.tabuWords
      )
      : this.gameService.submitPrompt({
        gameSessionId: this.currentGame.id,
        prompt: this.currentPrompt
      });

    submitObservable.subscribe({
      next: (result) => {
        this.lastResult = result;
        this.submitting = false;

        if (this.currentGame) {
          this.currentGame.score += result.score;
          this.currentGame.userPrompt = this.currentPrompt;
          this.currentGame.aiResponse = result.aiGuess;
          this.currentGame.isCorrectGuess = result.isCorrect;

          if (result.gameCompleted) {
            this.gameService.completeGame();
            this.stopGameTimer();
          } else {
            this.currentGame.attemptNumber++;
          }
        }
      },
      error: (error) => {
        console.error('Submit prompt error:', error);
        this.submitting = false;
      }
    });
  }

  continueGame() {
    this.lastResult = null;
    this.currentPrompt = '';
  }

  startNewGame() {
    this.gameState = 'idle';
    this.currentGame = null;
    this.lastResult = null;
    this.currentPrompt = '';
    this.gameService.setCurrentGame(null);
    this.stopGameTimer();
  }

  goHome() {
    window.location.href = '/';
  }

  clearPrompt() {
    this.currentPrompt = '';
  }

  getDifficultyText(level: number): string {
    const levels = { 1: 'Kolay', 2: 'Orta', 3: 'Zor', 4: 'Uzman' };
    return levels[level as keyof typeof levels] || 'Bilinmiyor';
  }

  getQualityText(quality: number): string {
    const qualities = {
      1: '⭐ Zayıf',
      2: '⭐⭐ Vasat',
      3: '⭐⭐⭐ İyi',
      4: '⭐⭐⭐⭐ Çok İyi',
      5: '⭐⭐⭐⭐⭐ Mükemmel'
    };
    return qualities[quality as keyof typeof qualities] || 'Bilinmiyor';
  }

  private startGameTimer() {
    this.startTime = new Date();
    this.gameTimer = interval(1000).subscribe(() => {
      this.updateGameTime();
    });
  }

  private stopGameTimer() {
    if (this.gameTimer) {
      this.gameTimer.unsubscribe();
      this.gameTimer = null;
    }
  }

  private updateGameTime() {
    if (!this.startTime) return;

    const now = new Date();
    const elapsed = Math.floor((now.getTime() - this.startTime.getTime()) / 1000);
    const minutes = Math.floor(elapsed / 60);
    const seconds = elapsed % 60;

    this.gameTime = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
  }
}