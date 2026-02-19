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
              <span *ngFor="let a of [1,2,3]; let i = index" class="attempt-dot" [class.used]="i < currentGame.attemptNumber - 1"></span>
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

        <!-- Input Area -->
        <div class="input-area glass-card" [class.input-error]="inputError">
          <textarea 
            #promptInput
            [(ngModel)]="currentPrompt" 
            class="game-input" 
            placeholder="Yasaklı kelimeleri kullanmadan hedefini anlat..."
            [disabled]="submitting"
            (keydown.enter)="onEnter($event)"
            (input)="checkTabuWords()"
            rows="1"></textarea>
          
          <button class="btn btn-send" (click)="submitPrompt()" [disabled]="!currentPrompt.trim() || submitting">
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
        <div class="result-modal glass-card pop-in" [class.success]="lastResult.isCorrect" [class.fail]="!lastResult.isCorrect">
          
          <!-- Confetti Effect -->
          <div *ngIf="lastResult.isCorrect" class="confetti-container">
            <div *ngFor="let c of confettiPieces" class="confetti" [style]="c"></div>
          </div>

          <div class="result-icon" [class.bounce]="lastResult.isCorrect">
            {{ lastResult.isCorrect ? '🎉' : '😔' }}
          </div>
          
          <h2 class="result-title">{{ lastResult.isCorrect ? 'MÜKEMMEL!' : 'OLMADI...' }}</h2>
          <p class="result-msg">{{ lastResult.isCorrect ? 'AI doğru tahmin etti!' : 'AI tahmin edemedi.' }}</p>

          <div class="ai-guess-box">
            <span class="label">AI Tahmini:</span>
            <div class="guess-value">"{{ lastResult.aiGuess }}"</div>
          </div>

          <!-- Score Display -->
          <div *ngIf="lastResult.score > 0" class="score-display pop-in">
            <span class="score-label">+</span>
            <span class="score-number">{{ lastResult.score }}</span>
            <span class="score-label">puan</span>
          </div>

          <!-- Prompt Quality -->
          <div class="quality-section">
            <span class="quality-label">Prompt Kalitesi</span>
            <div class="quality-stars">
              <span *ngFor="let s of [1,2,3,4,5]" 
                    class="star" 
                    [class.filled]="s <= lastResult.promptQuality">★</span>
            </div>
          </div>

          <div class="feedback-section">
            <p class="feedback-text">{{ lastResult.aiFeedback }}</p>
            
            <div *ngIf="lastResult.suggestions.length > 0" class="suggestions-box">
              <h4>💡 İpuçları</h4>
              <ul>
                <li *ngFor="let suggestion of lastResult.suggestions">{{ suggestion }}</li>
              </ul>
            </div>
          </div>

          <div class="modal-actions">
            <button *ngIf="!lastResult.gameCompleted && (currentGame?.attemptNumber ?? 0) < 3" 
              class="btn btn-primary" (click)="continueGame()">
              🔄 Tekrar Dene
            </button>
            <button class="btn btn-secondary" (click)="startNewGame()">
              🎮 Yeni Oyun
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .game-container {
      padding-bottom: 100px;
      min-height: 80vh;
      display: flex;
      flex-direction: column;
      justify-content: center;
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

    .spinner-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 24px;
    }

    .loading-ring {
      display: inline-block;
      position: relative;
      width: 64px;
      height: 64px;
    }
    .loading-ring div {
      box-sizing: border-box;
      display: block;
      position: absolute;
      width: 52px;
      height: 52px;
      margin: 6px;
      border: 4px solid transparent;
      border-radius: 50%;
      animation: ring 1.2s cubic-bezier(0.5, 0, 0.5, 1) infinite;
      border-color: #8b5cf6 transparent transparent transparent;
    }
    .loading-ring div:nth-child(1) { animation-delay: -0.45s; }
    .loading-ring div:nth-child(2) { animation-delay: -0.3s; }
    .loading-ring div:nth-child(3) { animation-delay: -0.15s; }
    @keyframes ring {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .loading-text {
      color: #c4b5fd;
      font-size: 1.1rem;
      font-weight: 600;
      letter-spacing: 1px;
      animation: pulse-text 1.5s ease-in-out infinite;
    }

    @keyframes pulse-text {
      0%, 100% { opacity: 0.6; }
      50% { opacity: 1; }
    }

    /* ===== Start Screen ===== */
    .start-panel {
      padding: 40px;
      max-width: 650px;
      margin: 0 auto;
    }

    .start-header {
      text-align: center;
      margin-bottom: 32px;
    }

    .start-icon {
      font-size: 3.5rem;
      margin-bottom: 16px;
    }

    .start-header h1 {
      font-size: 1.8rem;
      font-weight: 700;
      margin-bottom: 4px;
    }

    .mode-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
    }

    .mode-card {
      background: rgba(255,255,255,0.03);
      border: 2px solid rgba(255,255,255,0.08);
      padding: 28px 20px;
      border-radius: 20px;
      cursor: pointer;
      text-align: center;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      position: relative;
      overflow: hidden;
    }

    .mode-card::before {
      content: '';
      position: absolute;
      top: 0; left: 0; right: 0; bottom: 0;
      background: radial-gradient(circle at 50% 0%, rgba(139, 92, 246, 0.15), transparent 70%);
      opacity: 0;
      transition: opacity 0.3s;
    }

    .mode-card:hover::before,
    .mode-card.active::before { opacity: 1; }

    .mode-card.active {
      border-color: var(--primary);
      transform: scale(1.02);
      box-shadow: 0 0 30px rgba(139, 92, 246, 0.15);
    }

    .mode-icon-wrapper {
      margin-bottom: 16px;
    }

    .mode-icon { 
      font-size: 2.8rem; 
      transition: transform 0.3s;
    }

    .mode-card:hover .mode-icon { transform: scale(1.1) rotate(-5deg); }

    .mode-card h3 { 
      font-size: 1.1rem; 
      font-weight: 700; 
      margin-bottom: 6px;
      position: relative;
    }
    .mode-card p { 
      font-size: 0.85rem; 
      color: var(--text-muted); 
      line-height: 1.4;
      position: relative; 
    }

    .mode-badge {
      display: inline-block;
      margin-top: 12px;
      padding: 3px 12px;
      border-radius: 20px;
      font-size: 0.75rem;
      font-weight: 700;
      letter-spacing: 0.5px;
      position: relative;
    }
    .mode-badge.free { background: rgba(16, 185, 129, 0.2); color: #6ee7b7; }
    .mode-badge.ai { background: rgba(139, 92, 246, 0.2); color: #c4b5fd; }

    /* Category Section */
    .category-section {
      margin-top: 28px;
    }
    .section-label {
      font-size: 0.85rem;
      color: var(--text-muted);
      margin-bottom: 12px;
      font-weight: 600;
    }
    .category-chips {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
    }
    .chip {
      background: rgba(255, 255, 255, 0.05);
      border: 1px solid rgba(255, 255, 255, 0.1);
      color: var(--text-muted);
      padding: 6px 16px;
      border-radius: 20px;
      font-size: 0.85rem;
      cursor: pointer;
      transition: all 0.2s;
    }
    .chip:hover { background: rgba(255, 255, 255, 0.1); color: white; }
    .chip.active { 
      background: rgba(139, 92, 246, 0.2); 
      border-color: var(--primary); 
      color: #c4b5fd; 
    }

    .start-btn {
      position: relative;
      overflow: hidden;
      padding: 16px 40px;
    }
    .start-btn .btn-arrow {
      display: inline-block;
      transition: transform 0.3s;
      margin-left: 8px;
    }
    .start-btn:hover .btn-arrow { transform: translateX(4px); }

    /* ===== Game Header ===== */
    .game-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 24px;
      margin-bottom: 24px;
    }

    .stat-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 4px;
    }

    .stat-item .label {
      font-size: 0.7rem;
      color: var(--text-muted);
      letter-spacing: 1.5px;
      font-weight: 600;
    }

    .stat-item .value {
      font-size: 1.25rem;
      font-weight: 700;
      font-family: 'JetBrains Mono', monospace;
    }

    .score-glow {
      color: var(--accent);
      text-shadow: 0 0 12px rgba(16, 185, 129, 0.5);
    }

    .score-bump {
      animation: bump 0.3s ease;
    }

    @keyframes bump {
      0% { transform: scale(1); }
      50% { transform: scale(1.3); color: #fbbf24; }
      100% { transform: scale(1); }
    }

    .attempt-dot {
      display: inline-block;
      width: 10px;
      height: 10px;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.15);
      margin: 0 3px;
      transition: all 0.3s;
    }
    .attempt-dot.used {
      background: var(--danger);
      box-shadow: 0 0 6px rgba(239, 68, 68, 0.5);
    }

    .timer-ring {
      font-size: 1.4rem;
      font-weight: 700;
      color: white;
      background: rgba(255,255,255,0.08);
      padding: 6px 20px;
      border-radius: 24px;
      font-family: 'JetBrains Mono', monospace;
      border: 1px solid rgba(255,255,255,0.1);
      transition: all 0.3s;
    }
    .timer-warning {
      color: #fbbf24;
      border-color: rgba(251, 191, 36, 0.3);
      animation: timer-pulse 1s ease infinite;
    }
    @keyframes timer-pulse {
      0%, 100% { box-shadow: none; }
      50% { box-shadow: 0 0 12px rgba(251, 191, 36, 0.3); }
    }

    /* ===== Card Area ===== */
    .card-area {
      flex: 1;
      display: flex;
      justify-content: center;
      perspective: 1000px;
      margin-bottom: 24px;
    }

    .target-card {
      width: 100%;
      max-width: 420px;
      background: linear-gradient(145deg, rgba(30, 41, 59, 0.95), rgba(15, 23, 42, 0.98));
      border: 1px solid rgba(255, 255, 255, 0.12);
      padding: 0;
      overflow: hidden;
      box-shadow: 0 20px 50px rgba(0,0,0,0.4), 0 0 40px rgba(139, 92, 246, 0.05);
      transition: transform 0.3s;
    }

    .card-shake {
      animation: shake 0.5s ease;
    }

    @keyframes shake {
      0%, 100% { transform: translateX(0); }
      20% { transform: translateX(-8px); }
      40% { transform: translateX(8px); }
      60% { transform: translateX(-4px); }
      80% { transform: translateX(4px); }
    }

    .card-header {
      display: flex;
      justify-content: space-between;
      padding: 14px 18px;
      background: rgba(0,0,0,0.25);
    }

    .difficulty-badge {
      background: #4b5563;
      padding: 4px 10px;
      border-radius: 6px;
      font-size: 0.75rem;
      font-weight: 700;
      letter-spacing: 0.5px;
    }
    .difficulty-badge[data-level="1"] { background: var(--accent); color: #000; }
    .difficulty-badge[data-level="2"] { background: #eab308; color: #000; }
    .difficulty-badge[data-level="3"] { background: #f97316; color: #000; }
    .difficulty-badge[data-level="4"] { background: var(--danger); color: white; }

    .category-badge {
      background: rgba(139, 92, 246, 0.2);
      color: #c4b5fd;
      padding: 4px 10px;
      border-radius: 6px;
      font-size: 0.75rem;
      font-weight: 600;
    }

    .word-display {
      text-align: center;
      padding: 36px 20px;
      border-bottom: 1px solid rgba(255,255,255,0.08);
      position: relative;
    }

    .word-display::before {
      content: '';
      position: absolute;
      top: 0; left: 0; right: 0; bottom: 0;
      background: radial-gradient(circle at 50% 50%, rgba(139, 92, 246, 0.06), transparent 70%);
    }

    .target-word {
      font-size: 2.5rem;
      color: white;
      text-transform: uppercase;
      letter-spacing: 3px;
      margin: 0;
      text-shadow: 0 2px 8px rgba(0,0,0,0.5);
      position: relative;
    }

    .slide-up {
      animation: slideUp 0.5s ease forwards;
    }

    @keyframes slideUp {
      from { opacity: 0; transform: translateY(20px); }
      to { opacity: 1; transform: translateY(0); }
    }

    .tabu-section {
      padding: 20px;
      background: rgba(239, 68, 68, 0.04);
    }

    .tabu-label {
      text-align: center;
      color: var(--danger);
      font-size: 0.75rem;
      font-weight: 700;
      margin-bottom: 14px;
      letter-spacing: 1.5px;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 6px;
    }
    .tabu-icon { font-size: 0.9rem; }

    .tabu-list {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }

    .tabu-item {
      background: rgba(239, 68, 68, 0.08);
      color: #fca5a5;
      padding: 10px;
      text-align: center;
      border-radius: 10px;
      font-weight: 600;
      border: 1px solid rgba(239, 68, 68, 0.15);
      animation: slideUp 0.4s ease forwards;
      opacity: 0;
      font-size: 0.95rem;
    }

    /* ===== Input Area ===== */
    .input-area {
      display: flex;
      gap: 12px;
      padding: 14px;
      align-items: flex-end;
      max-width: 420px;
      margin: 0 auto;
      width: 100%;
      transition: border-color 0.3s;
    }

    .input-error {
      border-color: rgba(239, 68, 68, 0.5) !important;
      animation: shake 0.3s ease;
    }

    .game-input {
      flex: 1;
      background: transparent;
      border: none;
      color: white;
      resize: none;
      font-family: 'Poppins', sans-serif;
      font-size: 1rem;
      min-height: 24px;
      max-height: 100px;
      padding: 10px;
      line-height: 1.5;
    }
    .game-input::placeholder {
      color: rgba(148, 163, 184, 0.6);
    }
    .game-input:focus { outline: none; }

    .btn-send {
      background: linear-gradient(135deg, var(--primary), #7c3aed);
      color: white;
      width: 48px;
      height: 48px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.2rem;
      padding: 0;
      border: none;
      cursor: pointer;
      transition: all 0.3s;
      flex-shrink: 0;
    }
    .btn-send:hover:not(:disabled) {
      transform: scale(1.1);
      box-shadow: 0 4px 20px rgba(139, 92, 246, 0.4);
    }
    .btn-send:disabled {
      background: #475569;
      cursor: not-allowed;
    }
    .send-icon {
      transition: transform 0.2s;
    }
    .btn-send:hover:not(:disabled) .send-icon {
      transform: translateX(2px);
    }

    /* Tabu Warning */
    .tabu-warning {
      text-align: center;
      color: #fbbf24;
      font-size: 0.9rem;
      padding: 10px;
      margin-top: 8px;
      background: rgba(251, 191, 36, 0.1);
      border-radius: 12px;
      border: 1px solid rgba(251, 191, 36, 0.2);
      max-width: 420px;
      margin-left: auto;
      margin-right: auto;
    }

    /* ===== Result Modal ===== */
    .result-modal-backdrop {
      position: fixed;
      top: 0; left: 0; right: 0; bottom: 0;
      background: rgba(0,0,0,0.85);
      z-index: 1500;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 20px;
      backdrop-filter: blur(10px);
    }

    .result-modal {
      width: 100%;
      max-width: 480px;
      padding: 40px 32px;
      text-align: center;
      background: linear-gradient(145deg, #1e293b, #0f172a);
      border: 2px solid transparent;
      position: relative;
      overflow: hidden;
    }

    .result-modal.success { 
      border-color: rgba(16, 185, 129, 0.5);
      box-shadow: 0 0 40px rgba(16, 185, 129, 0.1);
    }
    .result-modal.fail { 
      border-color: rgba(239, 68, 68, 0.3);
    }

    /* Confetti */
    .confetti-container {
      position: absolute;
      top: 0; left: 0; right: 0; bottom: 0;
      pointer-events: none;
      overflow: hidden;
    }
    .confetti {
      position: absolute;
      width: 10px;
      height: 10px;
      border-radius: 2px;
      animation: confettiFall 2s ease-out forwards;
    }
    @keyframes confettiFall {
      0% { transform: translateY(-100px) rotate(0deg); opacity: 1; }
      100% { transform: translateY(500px) rotate(720deg); opacity: 0; }
    }

    .result-icon { 
      font-size: 4.5rem; 
      margin-bottom: 16px;
      display: inline-block;
    }

    .bounce {
      animation: bounceIn 0.6s cubic-bezier(0.68, -0.55, 0.265, 1.55);
    }
    @keyframes bounceIn {
      0% { transform: scale(0); opacity: 0; }
      50% { transform: scale(1.3); }
      100% { transform: scale(1); opacity: 1; }
    }

    .result-title {
      font-size: 1.8rem;
      font-weight: 800;
      margin-bottom: 4px;
      letter-spacing: 1px;
    }
    .result-msg {
      color: var(--text-muted);
      margin-bottom: 20px;
    }

    .ai-guess-box {
      margin: 20px 0;
      padding: 18px;
      background: rgba(255,255,255,0.04);
      border-radius: 14px;
      border: 1px solid rgba(255,255,255,0.08);
    }
    .ai-guess-box .label {
      font-size: 0.8rem;
      color: var(--text-muted);
      display: block;
      margin-bottom: 6px;
    }
    .guess-value {
      font-size: 1.4rem;
      font-weight: 700;
      color: var(--primary);
    }

    /* Score Display */
    .score-display {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      background: rgba(16, 185, 129, 0.15);
      border: 1px solid rgba(16, 185, 129, 0.3);
      padding: 8px 24px;
      border-radius: 24px;
      margin: 12px 0;
    }
    .score-number {
      font-size: 2rem;
      font-weight: 800;
      color: #6ee7b7;
      font-family: 'JetBrains Mono', monospace;
    }
    .score-label {
      font-size: 0.9rem;
      color: #6ee7b7;
      font-weight: 600;
    }

    /* Quality Stars */
    .quality-section {
      margin: 16px 0;
    }
    .quality-label {
      font-size: 0.8rem;
      color: var(--text-muted);
      display: block;
      margin-bottom: 6px;
    }
    .quality-stars {
      display: flex;
      justify-content: center;
      gap: 4px;
    }
    .star {
      font-size: 1.4rem;
      color: rgba(255, 255, 255, 0.15);
      transition: color 0.2s;
    }
    .star.filled {
      color: #eab308;
      text-shadow: 0 0 8px rgba(234, 179, 8, 0.5);
    }

    /* Feedback */
    .feedback-section {
      margin-top: 16px;
    }
    .feedback-text {
      color: var(--text-muted);
      font-size: 0.95rem;
      line-height: 1.5;
      margin-bottom: 12px;
    }
    .suggestions-box {
      background: rgba(255, 255, 255, 0.03);
      border: 1px solid rgba(255, 255, 255, 0.08);
      border-radius: 12px;
      padding: 16px;
      text-align: left;
      margin-top: 12px;
    }
    .suggestions-box h4 {
      font-size: 0.9rem;
      margin-bottom: 10px;
      color: #c4b5fd;
    }
    .suggestions-box ul {
      list-style: none;
      padding: 0;
    }
    .suggestions-box li {
      padding: 6px 0;
      color: var(--text-muted);
      font-size: 0.85rem;
      padding-left: 16px;
      position: relative;
    }
    .suggestions-box li::before {
      content: '→';
      position: absolute;
      left: 0;
      color: var(--primary);
    }

    .modal-actions {
      display: flex;
      gap: 12px;
      justify-content: center;
      margin-top: 28px;
    }

    /* ===== Responsive ===== */
    @media (max-width: 768px) {
      .mode-grid { grid-template-columns: 1fr; }
      .game-container { padding-top: 20px; }
      .start-panel { padding: 28px 20px; }
      .target-word { font-size: 2rem; }
      .result-modal { padding: 28px 20px; }
      .modal-actions { flex-direction: column; }
      .modal-actions .btn { width: 100%; }
    }
  `]
})
export class GameComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('promptInput') promptInput!: ElementRef;

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
    this.gameService.setCurrentGame(null);
    this.gameService.gameState$.subscribe(state => this.gameState = state);
    this.gameService.currentGame$.subscribe(game => {
      this.currentGame = game;
      if (game && this.gameState === 'playing') this.startGameTimer();
    });
  }

  ngOnDestroy() {
    this.stopGameTimer();
  }

  ngAfterViewChecked() {
    // Auto resize textarea if needed
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
    const promptLower = this.currentPrompt.toLowerCase();
    for (const tabu of this.currentGame.word.tabuWords) {
      if (promptLower.includes(tabu.toLowerCase())) {
        this.detectedTabuWord = tabu;
        break;
      }
    }
  }

  submitPrompt() {
    if (!this.currentGame || !this.currentPrompt.trim() || this.submitting) return;

    // Check for tabu words client-side
    if (this.detectedTabuWord) {
      this.inputError = true;
      this.cardShake = true;
      this.toastService.warning(`"${this.detectedTabuWord}" yasaklı bir kelime! Kullanmamalısın.`);
      setTimeout(() => { this.inputError = false; this.cardShake = false; }, 500);
      return;
    }

    this.submitting = true;
    this.loadingText = 'AI Düşünüyor...';

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

          if (result.score > 0) {
            this.scoreBump = true;
            setTimeout(() => this.scoreBump = false, 400);
          }

          if (result.gameCompleted) {
            this.gameService.completeGame();
            this.stopGameTimer();
            this.toastService.success(`Tebrikler! +${result.score} puan kazandın! 🎉`);
            this.generateConfetti();
          } else {
            this.currentGame.attemptNumber++;
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
    this.stopGameTimer();
  }

  onBackdropClick(event: MouseEvent) {
    if ((event.target as HTMLElement).classList.contains('result-modal-backdrop')) {
      // Allow dismissing by clicking backdrop only if game completed
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
    this.confettiPieces = Array.from({ length: 30 }, (_, i) => {
      const color = colors[i % colors.length];
      const left = Math.random() * 100;
      const delay = Math.random() * 0.5;
      const size = Math.random() * 8 + 6;
      return `left: ${left}%; background: ${color}; animation-delay: ${delay}s; width: ${size}px; height: ${size}px; top: -20px;`;
    });
  }

  private startGameTimer() {
    this.startTime = new Date();
    this.isTimerWarning = false;
    this.gameTimer = interval(1000).subscribe(() => this.updateGameTime());
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

    // Warning after 2 minutes
    if (elapsed > 120) {
      this.isTimerWarning = true;
    }
  }
}