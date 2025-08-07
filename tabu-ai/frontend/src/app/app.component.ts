import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  template: `
    <div class="app-container">
      <header class="app-header">
        <div class="container">
          <nav class="navbar">
            <div class="navbar-brand">
              <h1>🎯 TABU.AI</h1>
              <p class="tagline">Prompt Engineering Oyunu</p>
            </div>
            <div class="navbar-menu">
              <a routerLink="/" class="nav-link">Ana Sayfa</a>
              <a routerLink="/game" class="nav-link">Oyun</a>
              <a routerLink="/leaderboard" class="nav-link">Liderlik</a>
            </div>
          </nav>
        </div>
      </header>

      <main class="app-main">
        <router-outlet></router-outlet>
      </main>

      <footer class="app-footer">
        <div class="container">
          <div class="footer-content">
            <div class="footer-section">
              <h3>TABU.AI</h3>
              <p>Yapay zeka ile etkili iletişim kurma becerilerinizi geliştirin!</p>
            </div>
            <div class="footer-section">
              <h4>Özellikler</h4>
              <ul>
                <li>🎯 Interaktif oyun deneyimi</li>
                <li>🤖 AI tabanlı değerlendirme</li>
                <li>📈 İlerleme takibi</li>
                <li>🏆 Gamification sistemi</li>
              </ul>
            </div>
            <div class="footer-section">
              <h4>İletişim</h4>
              <p>© 2024 TABU.AI</p>
              <p>"Yasaklı kelimeler, sınırsız yaratıcılık!"</p>
            </div>
          </div>
        </div>
      </footer>
    </div>
  `,
  styles: [`
    .app-container {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
    }

    .app-header {
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(20px);
      border-bottom: 1px solid rgba(255, 255, 255, 0.2);
      position: sticky;
      top: 0;
      z-index: 1000;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
    }

    .navbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 0;
    }

    .navbar-brand h1 {
      font-size: 28px;
      font-weight: bold;
      background: linear-gradient(45deg, #667eea, #764ba2);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      margin: 0;
    }

    .tagline {
      font-size: 14px;
      color: #6c757d;
      margin: 0;
    }

    .navbar-menu {
      display: flex;
      gap: 24px;
    }

    .nav-link {
      text-decoration: none;
      color: #333;
      font-weight: 500;
      padding: 8px 16px;
      border-radius: 8px;
      transition: all 0.3s ease;
    }

    .nav-link:hover {
      background: rgba(102, 126, 234, 0.1);
      color: #667eea;
      transform: translateY(-1px);
    }

    .app-main {
      flex: 1;
      padding: 40px 0;
    }

    .app-footer {
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(20px);
      border-top: 1px solid rgba(255, 255, 255, 0.2);
      padding: 40px 0;
      margin-top: auto;
    }

    .footer-content {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 32px;
    }

    .footer-section h3,
    .footer-section h4 {
      margin-bottom: 16px;
      color: #333;
    }

    .footer-section h3 {
      font-size: 24px;
      background: linear-gradient(45deg, #667eea, #764ba2);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .footer-section p {
      color: #6c757d;
      line-height: 1.6;
      margin-bottom: 8px;
    }

    .footer-section ul {
      list-style: none;
      padding: 0;
    }

    .footer-section li {
      color: #6c757d;
      margin-bottom: 8px;
      padding-left: 0;
    }

    @media (max-width: 768px) {
      .navbar {
        flex-direction: column;
        gap: 16px;
        text-align: center;
      }

      .navbar-menu {
        justify-content: center;
      }

      .footer-content {
        grid-template-columns: 1fr;
        text-align: center;
      }
    }
  `]
})
export class AppComponent {
  title = 'TABU.AI';
}