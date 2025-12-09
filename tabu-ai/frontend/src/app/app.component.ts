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
    </div>
  `,
  styles: [`
    .app-container {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
    }

    .app-header {
      background: white;
      border-bottom: 1px solid #e2e8f0;
      position: sticky;
      top: 0;
      z-index: 1000;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
    }

    .navbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 0;
    }

    .navbar-brand h1 {
      font-size: 24px;
      font-weight: 700;
      color: #667eea;
      margin: 0;
    }

    .navbar-menu {
      display: flex;
      gap: 8px;
    }

    .nav-link {
      text-decoration: none;
      color: #4a5568;
      font-weight: 500;
      font-size: 15px;
      padding: 8px 16px;
      border-radius: 6px;
      transition: all 0.2s ease;
    }

    .nav-link:hover {
      background: #f7fafc;
      color: #667eea;
    }

    .app-main {
      flex: 1;
      padding: 32px 0;
    }

    @media (max-width: 768px) {
      .navbar {
        flex-direction: column;
        gap: 12px;
        text-align: center;
      }

      .navbar-brand h1 {
        font-size: 20px;
      }

      .navbar-menu {
        width: 100%;
        justify-content: center;
      }

      .app-main {
        padding: 24px 0;
      }
    }
  `]
})
export class AppComponent {
  title = 'TABU.AI';
}