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
      <!-- Game not started -->
      <div *ngIf="gameState === 'idle'" class="game-start fade-in">
        <div class="start-card">
          <h1>🎯 TABU.AI Oyunu</h1>
          <p>Prompt engineering becerilerinizi test etmeye hazır mısınız?</p>
          
          <div class="game-modes">
            <h3>Oyun Modu Seçin:</h3>
            <div class="mode-options">
              <button 
                class="mode-btn"
                [class.selected]="selectedMode === 'demo'"
                (click)="selectedMode = 'demo'">
                🎮 Demo Modu
                <span class="mode-desc">Hemen başlayın, kayıt gerektirmez</span>
              </button>
              <button 
                class="mode-btn"
                [class.selected]="selectedMode === 'real'"
                (click)="selectedMode = 'real'">
                🤖 AI Modu
                <span class="mode-desc">Gerçek AI değerlendirmesi (API Key gerekli)</span>
              </button>
            </div>
          </div>

          <button 
            class="btn btn-primary btn-large pulse"
            (click)="startGame()"
            [disabled]="loading">
            <span *ngIf="loading" class="loading"></span>
            {{ loading ? 'Oyun Başlatılıyor...' : '🚀 Oyunu Başlat' }}
          </button>
        </div>
      </div>

      <!-- Game in progress -->
      <div *ngIf="gameState === 'playing' && currentGame" class="game-board fade-in">
        <!-- Game Header -->
        <div class="game-header">
          <div class="game-info">
            <h2>🎯 Oyun Devam Ediyor</h2>
            <div class="game-stats">
              <div class="stat">
                <span class="stat-label">Kategori:</span>
                <span class="stat-value">{{ currentGame.word.category }}</span>
              </div>
              <div class="stat">
                <span class="stat-label">Zorluk:</span>
                <span class="stat-value">{{ getDifficultyText(currentGame.word.difficulty) }}</span>
              </div>
              <div class="stat">
                <span class="stat-label">Deneme:</span>
                <span class="stat-value">{{ currentGame.attemptNumber }}/3</span>
              </div>
              <div class="stat">
                <span class="stat-label">Süre:</span>
                <span class="stat-value">{{ gameTime }}</span>
              </div>
            </div>
          </div>
          <div class="score-display">
            💎 {{ currentGame.score }} Puan
          </div>
        </div>

        <!-- Game Content -->
        <div class="game-content">
          <!-- Target Word -->
          <div class="word-section">
            <div class="target-word-card">
              <h3>🎯 Hedef Kelime</h3>
              <div class="target-word">{{ currentGame.word.targetWord }}</div>
              <p>Bu kelimeyi AI'ya tarif edin, ancak aşağıdaki yasaklı kelimeleri kullanmayın!</p>
            </div>

            <!-- Tabu Words -->
            <div class="tabu-section">
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

          <!-- Prompt Input -->
          <div class="prompt-section">
            <h3>✍️ Promptunuzu Yazın</h3>
            <p>AI'nın "{{ currentGame.word.targetWord }}" kelimesini tahmin edebilmesi için açıklayıcı bir prompt yazın.</p>
            
            <textarea
              [(ngModel)]="currentPrompt"
              class="prompt-input"
              placeholder="Örnek: Bu, insanların bir yerden başka bir yere gitmeyi sağlayan, metalden yapılmış, dört tekerlekli bir araçtır..."
              [disabled]="submitting"></textarea>

            <div class="prompt-actions">
              <button 
                class="btn btn-primary"
                (click)="submitPrompt()"
                [disabled]="!currentPrompt.trim() || submitting">
                <span *ngIf="submitting" class="loading"></span>
                {{ submitting ? 'AI Düşünüyor...' : '🤖 AI\'ya Gönder' }}
              </button>
              <button 
                class="btn btn-secondary"
                (click)="clearPrompt()"
                [disabled]="submitting">
                🗑️ Temizle
              </button>
            </div>

            <div class="prompt-tips">
              <h4>💡 İpuçları:</h4>
              <ul>
                <li>Yasaklı kelimeleri kullanmayın</li>
                <li>Spesifik ve açıklayıcı olun</li>
                <li>Nesnenin kullanım alanlarını belirtin</li>
                <li>Fiziksel özelliklerini tanımlayın</li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      <!-- Game Result -->
      <div *ngIf="lastResult" class="game-result fade-in">
        <div class="result-card" [class.success]="lastResult.isCorrect" [class.failure]="!lastResult.isCorrect">
          <div class="result-header">
            <div class="result-icon">{{ lastResult.isCorrect ? '🎉' : '😔' }}</div>
            <h2>{{ lastResult.isCorrect ? 'Tebrikler!' : 'Maalesef!' }}</h2>
            <p>{{ lastResult.isCorrect ? 'AI doğru tahmin etti!' : 'AI yanlış tahmin etti.' }}</p>
          </div>

          <div class="result-details">
            <div class="detail">
              <span class="detail-label">AI Tahmini:</span>
              <span class="detail-value ai-guess">{{ lastResult.aiGuess }}</span>
            </div>
            <div class="detail">
              <span class="detail-label">Kazanılan Puan:</span>
              <span class="detail-value score">+{{ lastResult.score }}</span>
            </div>
            <div class="detail">
              <span class="detail-label">Prompt Kalitesi:</span>
              <span class="detail-value quality">{{ getQualityText(lastResult.promptQuality) }}</span>
            </div>
          </div>

          <div class="ai-feedback">
            <h4>🤖 AI Geri Bildirimi:</h4>
            <p>{{ lastResult.aiFeedback }}</p>
          </div>

          <div *ngIf="lastResult.suggestions.length > 0" class="suggestions">
            <h4>💡 İyileştirme Önerileri:</h4>
            <ul>
              <li *ngFor="let suggestion of lastResult.suggestions">{{ suggestion }}</li>
            </ul>
          </div>

          <div class="result-actions">
            <button 
              *ngIf="!lastResult.gameCompleted && currentGame?.attemptNumber < 3"
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
    .game-start {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 70vh;
    }

    .start-card {
      background: white;
      border-radius: 20px;
      padding: 40px;
      text-align: center;
      max-width: 600px;
      width: 100%;
      box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
    }

    .start-card h1 {
      font-size: 2.5rem;
      margin-bottom: 16px;
      background: linear-gradient(45deg, #667eea, #764ba2);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .game-modes {
      margin: 32px 0;
    }

    .game-modes h3 {
      margin-bottom: 20px;
      color: #333;
    }

    .mode-options {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      margin-bottom: 32px;
    }

    .mode-btn {
      padding: 20px;
      border: 2px solid #e9ecef;
      border-radius: 12px;
      background: white;
      cursor: pointer;
      transition: all 0.3s ease;
      text-align: center;
    }

    .mode-btn.selected {
      border-color: #667eea;
      background: rgba(102, 126, 234, 0.1);
    }

    .mode-btn:hover {
      border-color: #667eea;
      transform: translateY(-2px);
    }

    .mode-desc {
      display: block;
      font-size: 0.9rem;
      color: #6c757d;
      margin-top: 8px;
    }

    .game-board {
      max-width: 1200px;
      margin: 0 auto;
    }

    .game-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(20px);
      border-radius: 20px;
      padding: 24px;
      margin-bottom: 32px;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
    }

    .game-info h2 {
      margin-bottom: 16px;
      color: #333;
    }

    .game-stats {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
      gap: 16px;
    }

    .stat {
      text-align: center;
    }

    .stat-label {
      display: block;
      font-size: 0.9rem;
      color: #6c757d;
    }

    .stat-value {
      display: block;
      font-weight: bold;
      color: #333;
      margin-top: 4px;
    }

    .game-content {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 32px;
    }

    .word-section, .prompt-section {
      background: white;
      border-radius: 16px;
      padding: 24px;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
    }

    .target-word-card {
      text-align: center;
      margin-bottom: 24px;
    }

    .target-word-card h3 {
      margin-bottom: 16px;
      color: #333;
    }

    .target-word {
      font-size: 2.5rem;
      font-weight: bold;
      color: #667eea;
      background: rgba(102, 126, 234, 0.1);
      border-radius: 12px;
      padding: 20px;
      margin: 16px 0;
    }

    .tabu-section h4 {
      margin-bottom: 16px;
      color: #333;
      text-align: center;
    }

    .tabu-words {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      justify-content: center;
    }

    .prompt-section h3 {
      margin-bottom: 16px;
      color: #333;
    }

    .prompt-input {
      width: 100%;
      min-height: 150px;
      margin: 16px 0;
    }

    .prompt-actions {
      display: flex;
      gap: 12px;
      margin: 16px 0;
    }

    .prompt-tips {
      background: rgba(102, 126, 234, 0.05);
      border-radius: 8px;
      padding: 16px;
      margin-top: 24px;
    }

    .prompt-tips h4 {
      margin-bottom: 12px;
      color: #333;
    }

    .prompt-tips ul {
      margin: 0;
      padding-left: 20px;
    }

    .prompt-tips li {
      margin-bottom: 8px;
      color: #6c757d;
    }

    .game-result {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 70vh;
    }

    .result-card {
      background: white;
      border-radius: 20px;
      padding: 40px;
      max-width: 600px;
      width: 100%;
      text-align: center;
      box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
    }

    .result-card.success {
      border: 3px solid #51cf66;
    }

    .result-card.failure {
      border: 3px solid #ff6b6b;
    }

    .result-header {
      margin-bottom: 32px;
    }

    .result-icon {
      font-size: 4rem;
      margin-bottom: 16px;
    }

    .result-header h2 {
      margin-bottom: 8px;
      color: #333;
    }

    .result-details {
      display: grid;
      gap: 16px;
      margin-bottom: 32px;
      text-align: left;
    }

    .detail {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 16px;
      background: #f8f9fa;
      border-radius: 8px;
    }

    .detail-label {
      font-weight: 500;
      color: #6c757d;
    }

    .detail-value {
      font-weight: bold;
    }

    .ai-guess {
      color: #667eea;
    }

    .score {
      color: #51cf66;
    }

    .ai-feedback, .suggestions {
      text-align: left;
      margin-bottom: 24px;
      padding: 16px;
      background: #f8f9fa;
      border-radius: 8px;
    }

    .ai-feedback h4, .suggestions h4 {
      margin-bottom: 12px;
      color: #333;
    }

    .suggestions ul {
      margin: 0;
      padding-left: 20px;
    }

    .suggestions li {
      margin-bottom: 8px;
      color: #6c757d;
    }

    .result-actions {
      display: flex;
      gap: 12px;
      justify-content: center;
      flex-wrap: wrap;
    }

    @media (max-width: 768px) {
      .mode-options {
        grid-template-columns: 1fr;
      }

      .game-header {
        flex-direction: column;
        gap: 20px;
        text-align: center;
      }

      .game-content {
        grid-template-columns: 1fr;
      }

      .game-stats {
        grid-template-columns: repeat(2, 1fr);
      }

      .result-actions {
        flex-direction: column;
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

  constructor(private gameService: GameService) {}

  ngOnInit() {
    // Reset game state when component loads
    this.gameService.setCurrentGame(null);
    
    // Subscribe to game state changes
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
      userId: 'demo-user', // In real app, get from auth service
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
        // Show error message
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
        
        // Update game state
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
    // Navigate to home - in real app use Router
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