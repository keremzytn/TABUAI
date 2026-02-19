import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ToastComponent } from './components/toast/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterLink, RouterLinkActive, ToastComponent],
  template: `
    <div class="app-layout">
      <!-- Desktop Header -->
      <header class="desktop-header glass-card">
        <div class="container nav-container">
          <div class="brand">
            <span class="logo-icon">🎯</span>
            <h1>TABU<span class="text-gradient">.AI</span></h1>
          </div>
          <nav class="desktop-nav">
            <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-link">
              Ana Sayfa
            </a>
            <a routerLink="/game" routerLinkActive="active" class="nav-link">
              Oyna
            </a>
            <a routerLink="/leaderboard" routerLinkActive="active" class="nav-link">
              Liderlik
            </a>
          </nav>
        </div>
      </header>

      <!-- Main Content -->
      <main class="main-content">
        <router-outlet></router-outlet>
      </main>

      <!-- Mobile Bottom Navigation -->
      <nav class="mobile-nav glass-card">
        <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="mobile-link">
          <span class="icon">🏠</span>
          <span class="label">Başla</span>
        </a>
        <a routerLink="/game" routerLinkActive="active" class="mobile-link">
          <span class="icon">🎮</span>
          <span class="label">Oyna</span>
        </a>
        <a routerLink="/leaderboard" routerLinkActive="active" class="mobile-link">
          <span class="icon">🏆</span>
          <span class="label">Liderler</span>
        </a>
      </nav>

      <!-- Toast Notifications -->
      <app-toast></app-toast>
    </div>
  `,
  styles: [`
    .app-layout {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
      position: relative;
    }

    /* Desktop Header */
    .desktop-header {
      position: sticky;
      top: 20px;
      margin: 0 20px;
      z-index: 100;
      border-radius: 16px;
      padding: 12px 0;
      display: none; /* Mobile first hidden */
    }

    @media (min-width: 768px) {
      .desktop-header {
        display: block;
      }
    }

    .nav-container {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .brand {
      display: flex;
      align-items: center;
      gap: 12px;
      padding-left: 16px;
    }

    .brand h1 {
      font-size: 24px;
      font-weight: 800;
      letter-spacing: -0.5px;
      margin: 0;
      color: white;
    }

    .logo-icon {
      font-size: 28px;
      animation: float 3s ease-in-out infinite;
    }

    .desktop-nav {
      display: flex;
      gap: 8px;
      padding-right: 16px;
    }

    .nav-link {
      color: var(--text-muted);
      text-decoration: none;
      padding: 10px 20px;
      border-radius: 12px;
      font-weight: 600;
      transition: all 0.2s ease;
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .nav-link:hover {
      background: rgba(255, 255, 255, 0.05);
      color: white;
    }

    .nav-link.active {
      background: rgba(139, 92, 246, 0.2);
      color: var(--primary);
      box-shadow: 0 0 15px rgba(139, 92, 246, 0.1);
    }

    /* Main Content */
    .main-content {
      flex: 1;
      padding: 20px 0 100px 0; /* Extra padding for bottom nav */
      width: 100%;
      max-width: 100%;
      overflow-x: hidden;
    }

    @media (min-width: 768px) {
      .main-content {
        padding-top: 40px;
        padding-bottom: 40px;
      }
    }

    /* Mobile Bottom Nav */
    .mobile-nav {
      position: fixed;
      bottom: 20px;
      left: 20px;
      right: 20px;
      height: 70px;
      display: flex;
      justify-content: space-around;
      align-items: center;
      z-index: 1000;
      border-radius: 20px;
    }

    @media (min-width: 768px) {
      .mobile-nav {
        display: none;
      }
    }

    .mobile-link {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      text-decoration: none;
      color: var(--text-muted);
      gap: 4px;
      padding: 8px 16px;
      border-radius: 12px;
      transition: all 0.2s;
    }

    .mobile-link .icon {
      font-size: 24px;
      margin-bottom: 2px;
      transition: transform 0.2s;
    }

    .mobile-link .label {
      font-size: 10px;
      font-weight: 600;
      letter-spacing: 0.5px;
    }

    .mobile-link.active {
      color: var(--primary);
    }

    .mobile-link.active .icon {
      transform: translateY(-4px);
    }

    .mobile-link:active {
      transform: scale(0.95);
    }

    @keyframes float {
      0%, 100% { transform: translateY(0); }
      50% { transform: translateY(-5px); }
    }
  `]
})
export class AppComponent {
  title = 'TABU.AI';
}