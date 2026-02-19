import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { Observable, map } from 'rxjs';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container home-container">
      <!-- Hero Section -->
      <section class="hero-section fade-in">
        <div class="hero-content">
          <div class="badge-new pop-in">🤖 AI Destekli Kelime Oyunu</div>
          <h1 class="hero-title">
            Prompt <br>
            <span class="text-gradient">Engineering</span>
          </h1>
          <p class="hero-description">
            Yapay zeka ile iletişim kurma sanatını bir oyuna dönüştürdük. 
            Yasaklı kelimeleri kullanmadan AI'ya hedefi anlatabilir misin?
          </p>
          <div class="hero-actions">
            <a routerLink="/game" class="btn btn-primary btn-lg pulse-hover">
              🎮 Oyuna Başla
            </a>
            
            <a routerLink="/leaderboard" class="btn btn-secondary btn-lg">
              🏆 Liderler
            </a>

            <!-- Logged In -->
            <ng-container *ngIf="(isLoggedIn$ | async); else guestTemplate">
               <a routerLink="/profile" class="btn btn-secondary btn-lg profile-btn">
                👤 Profil
              </a>
              <button (click)="logout()" class="btn btn-secondary btn-lg logout-btn">
                🚪 Çıkış
              </button>
            </ng-container>

            <!-- Guest -->
            <ng-template #guestTemplate>
              <a routerLink="/login" class="btn btn-secondary btn-lg">
                🔑 Giriş Yap
              </a>
              <a routerLink="/register" class="btn btn-secondary btn-lg register-btn">
                ✨ Kayıt Ol
              </a>
            </ng-template>

          </div>
        </div>
      </section>

      <!-- Features Grid -->
      <section class="features-section">
        <div class="features-grid">
          <div class="feature-card glass-card">
            <div class="card-icon gradient-1">🤖</div>
            <h3>AI Hakem</h3>
            <p>Promptlarınız saniyeler içinde analiz edilir ve puanlanır.</p>
          </div>

          <div class="feature-card glass-card">
            <div class="card-icon gradient-2">🎯</div>
            <h3>Zorlu Görevler</h3>
            <p>Her seviyede daha zorlu yasaklı kelimeler ve karmaşık hedefler.</p>
          </div>

          <div class="feature-card glass-card">
            <div class="card-icon gradient-3">📈</div>
            <h3>Gelişim</h3>
            <p>Prompt yazma becerinizi geliştirin ve seviye atlayın.</p>
          </div>
        </div>
      </section>

      <!-- Stats Section -->
      <section class="stats-section fade-in">
        <div class="stats-grid">
          <div class="stat-card glass-card">
            <div class="stat-value text-gradient">20+</div>
            <div class="stat-label">Kelime</div>
          </div>
          <div class="stat-card glass-card">
            <div class="stat-value text-gradient">10</div>
            <div class="stat-label">Kategori</div>
          </div>
          <div class="stat-card glass-card">
            <div class="stat-value text-gradient">4</div>
            <div class="stat-label">Zorluk Seviyesi</div>
          </div>
          <div class="stat-card glass-card">
            <div class="stat-value text-gradient">∞</div>
            <div class="stat-label">Eğlence</div>
          </div>
        </div>
      </section>

      <!-- How to Play -->
      <section class="how-section glass-card">
        <h2 class="section-title">Nasıl Oynanır?</h2>
        <div class="steps-container">
          <div class="step-item">
            <div class="step-number">1</div>
            <div class="step-text">
              <h4>Hedef Kartı Al</h4>
              <p>Yasaklı kelimelere dikkat et.</p>
            </div>
          </div>
          <div class="step-line"></div>
          <div class="step-item">
            <div class="step-number">2</div>
            <div class="step-text">
              <h4>Prompt Yaz</h4>
              <p>AI'yı doğru kelimeye yönlendir.</p>
            </div>
          </div>
          <div class="step-line"></div>
          <div class="step-item">
            <div class="step-number">3</div>
            <div class="step-text">
              <h4>Puan Topla</h4>
              <p>Liderlik tablosuna tırman.</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .home-container {
      padding-top: 20px;
      padding-bottom: 100px;
    }

    /* Hero Section */
    .hero-section {
      text-align: center;
      padding: 40px 0 60px;
      max-width: 800px;
      margin: 0 auto;
    }

    .badge-new {
      display: inline-block;
      background: rgba(139, 92, 246, 0.15);
      color: #c4b5fd;
      padding: 8px 20px;
      border-radius: 24px;
      font-size: 14px;
      font-weight: 600;
      margin-bottom: 28px;
      border: 1px solid rgba(139, 92, 246, 0.25);
    }

    .hero-title {
      font-size: 3.5rem;
      font-weight: 800;
      line-height: 1.1;
      margin-bottom: 24px;
      letter-spacing: -1px;
    }

    .hero-description {
      font-size: 1.1rem;
      line-height: 1.7;
      color: var(--text-muted);
      margin-bottom: 40px;
      max-width: 600px;
      margin-left: auto;
      margin-right: auto;
    }

    .hero-actions {
      display: flex;
      gap: 12px;
      justify-content: center;
      flex-wrap: wrap;
    }

    .btn-lg {
      padding: 16px 28px;
      font-size: 1rem;
    }

    .profile-btn {
      border-color: rgba(139, 92, 246, 0.4);
    }
    .logout-btn {
      border-color: rgba(239, 68, 68, 0.3);
    }
    .logout-btn:hover {
      border-color: rgba(239, 68, 68, 0.6) !important;
      background: rgba(239, 68, 68, 0.1) !important;
    }
    .register-btn {
      border-color: rgba(139, 92, 246, 0.4);
    }

    /* Features Grid */
    .features-section {
      margin-bottom: 50px;
    }

    .features-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 20px;
    }

    .feature-card {
      padding: 30px;
      transition: transform 0.3s ease, box-shadow 0.3s;
      position: relative;
      overflow: hidden;
    }

    .feature-card::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 4px;
      background: linear-gradient(90deg, var(--primary), var(--accent));
      opacity: 0;
      transition: opacity 0.3s;
    }

    .feature-card:hover {
      transform: translateY(-6px);
      box-shadow: 0 12px 40px rgba(0, 0, 0, 0.3);
    }

    .feature-card:hover::before {
      opacity: 1;
    }

    .card-icon {
      font-size: 48px;
      margin-bottom: 20px;
      width: 80px;
      height: 80px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 20px;
      background: rgba(255, 255, 255, 0.05);
    }

    .feature-card h3 {
      font-size: 1.25rem;
      margin-bottom: 12px;
      font-weight: 700;
    }

    .feature-card p {
      color: var(--text-muted);
      font-size: 0.95rem;
      line-height: 1.5;
    }

    /* Stats Section */
    .stats-section {
      margin-bottom: 50px;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: 16px;
    }

    .stat-card {
      padding: 24px;
      text-align: center;
      transition: transform 0.3s;
    }

    .stat-card:hover {
      transform: translateY(-4px);
    }

    .stat-value {
      font-size: 2.5rem;
      font-weight: 800;
      margin-bottom: 4px;
      font-family: 'JetBrains Mono', monospace;
    }

    .stat-label {
      font-size: 0.85rem;
      color: var(--text-muted);
      font-weight: 500;
    }

    /* How Section */
    .how-section {
      padding: 40px;
      text-align: center;
    }

    .section-title {
      font-size: 2rem;
      font-weight: 700;
      margin-bottom: 40px;
    }

    .steps-container {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      position: relative;
    }

    .step-item {
      flex: 1;
      display: flex;
      flex-direction: column;
      align-items: center;
      z-index: 2;
    }

    .step-number {
      width: 56px;
      height: 56px;
      background: var(--bg-dark);
      border: 2px solid var(--primary);
      color: var(--primary);
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 700;
      font-size: 1.2rem;
      margin-bottom: 16px;
      box-shadow: 0 0 20px rgba(139, 92, 246, 0.2);
      transition: all 0.3s;
    }

    .step-item:hover .step-number {
      background: var(--primary);
      color: white;
      transform: scale(1.1);
    }

    .step-text h4 {
      font-size: 1.1rem;
      margin-bottom: 8px;
      font-weight: 700;
    }

    .step-text p {
      color: var(--text-muted);
      font-size: 0.9rem;
      line-height: 1.5;
    }

    .step-line {
      flex: 1;
      height: 2px;
      background: linear-gradient(90deg, rgba(139, 92, 246, 0.3), rgba(16, 185, 129, 0.3));
      margin-top: 28px;
    }

    /* Mobile Responsive */
    @media (max-width: 768px) {
      .hero-title {
        font-size: 2.5rem;
      }

      .hero-actions {
        flex-direction: column;
        width: 100%;
      }

      .btn {
        width: 100%;
      }

      .stats-grid {
        grid-template-columns: repeat(2, 1fr);
      }

      .steps-container {
        flex-direction: column;
        gap: 30px;
      }

      .step-line {
        display: none;
      }

      .step-item {
        flex-direction: row;
        gap: 16px;
        text-align: left;
        width: 100%;
      }

      .step-number {
        margin-bottom: 0;
        flex-shrink: 0;
      }
    }
  `]
})
export class HomeComponent {
  isLoggedIn$: Observable<boolean>;

  constructor(
    private authService: AuthService,
    private toastService: ToastService
  ) {
    this.isLoggedIn$ = this.authService.currentUser.pipe(map(user => !!user));
  }

  logout() {
    this.authService.logout();
    this.toastService.info('Başarıyla çıkış yapıldı.');
  }
}