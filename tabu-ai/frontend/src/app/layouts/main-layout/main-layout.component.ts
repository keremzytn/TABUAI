import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterLink, RouterLinkActive],
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
            <a routerLink="/versus" routerLinkActive="active" class="nav-link">
              Duello
            </a>
            <a routerLink="/challenges" routerLinkActive="active" class="nav-link">
              Meydan Okuma
            </a>
            <a routerLink="/activity" routerLinkActive="active" class="nav-link">
              Akis
            </a>
            <a routerLink="/friends" routerLinkActive="active" class="nav-link">
              Arkadas
            </a>
            <a *ngIf="!(authService.currentUser | async)" routerLink="/login" routerLinkActive="active" class="nav-link">
              Giriş
            </a>
            <a *ngIf="(authService.currentUser | async)" routerLink="/profile" routerLinkActive="active" class="nav-link">
              Profil
            </a>
          </nav>
        </div>
      </header>

      <!-- Main Content -->
      <main class="main-content">
        <router-outlet></router-outlet>
      </main>

      <!-- Mobile Bottom Navigation -->
      <nav class="mobile-nav">
        <div class="mobile-nav-bg"></div>
        <div class="mobile-nav-items">
          <!-- Ana Sayfa -->
          <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="mobile-link">
            <div class="icon-wrap">
              <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"></path>
                <polyline points="9 22 9 12 15 12 15 22"></polyline>
              </svg>
            </div>
            <span class="label">Ana Sayfa</span>
            <span class="active-dot"></span>
          </a>

          <!-- Liderlik -->
          <a routerLink="/leaderboard" routerLinkActive="active" class="mobile-link">
            <div class="icon-wrap">
              <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M6 9H4.5a2.5 2.5 0 0 1 0-5C7 4 7 7 7 7"></path>
                <path d="M18 9h1.5a2.5 2.5 0 0 0 0-5C17 4 17 7 17 7"></path>
                <path d="M4 22h16"></path>
                <path d="M10 14.66V17c0 .55-.47.98-.97 1.21C7.85 18.75 7 20 7 22"></path>
                <path d="M14 14.66V17c0 .55.47.98.97 1.21C16.15 18.75 17 20 17 22"></path>
                <path d="M18 2H6v7a6 6 0 0 0 12 0V2Z"></path>
              </svg>
            </div>
            <span class="label">Liderler</span>
            <span class="active-dot"></span>
          </a>

          <!-- PLAY - Center Floating Button -->
          <a routerLink="/game" routerLinkActive="active" class="mobile-link play-btn">
            <div class="play-btn-inner">
              <svg xmlns="http://www.w3.org/2000/svg" width="26" height="26" viewBox="0 0 24 24" fill="currentColor" stroke="none">
                <path d="M8 5.14v14l11-7-11-7z"/>
              </svg>
            </div>
            <span class="label play-label">Oyna</span>
          </a>

          <!-- Duello -->
          <a routerLink="/versus" routerLinkActive="active" class="mobile-link">
            <div class="icon-wrap">
              <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M14.5 17.5L3 6V3h3l11.5 11.5"></path>
                <path d="M13 19l6-6"></path>
                <path d="M16 16l4 4"></path>
                <path d="M19 21l2-2"></path>
                <path d="M14.5 6.5L18 3h3v3l-3.5 3.5"></path>
              </svg>
            </div>
            <span class="label">Duello</span>
            <span class="active-dot"></span>
          </a>

          <!-- Profil -->
          <a routerLink="/profile" routerLinkActive="active" class="mobile-link">
            <div class="icon-wrap">
              <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                <circle cx="12" cy="7" r="4"></circle>
              </svg>
            </div>
            <span class="label">Profil</span>
            <span class="active-dot"></span>
          </a>
        </div>
      </nav>
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
      display: none;
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
      padding: 20px 0 120px 0;
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

    /* ============================================ */
    /* Mobile Bottom Nav - Premium Design           */
    /* ============================================ */
    .mobile-nav {
      position: fixed;
      bottom: 0;
      left: 0;
      right: 0;
      z-index: 1000;
      padding-bottom: env(safe-area-inset-bottom, 8px);
    }

    @media (min-width: 768px) {
      .mobile-nav {
        display: none;
      }
    }

    .mobile-nav-bg {
      position: absolute;
      inset: 0;
      background: rgba(15, 23, 42, 0.85);
      backdrop-filter: blur(20px) saturate(180%);
      -webkit-backdrop-filter: blur(20px) saturate(180%);
      border-top: 1px solid rgba(139, 92, 246, 0.15);
      box-shadow:
        0 -4px 30px rgba(0, 0, 0, 0.3),
        0 -1px 0 rgba(139, 92, 246, 0.1);
    }

    .mobile-nav-items {
      position: relative;
      display: flex;
      justify-content: space-around;
      align-items: flex-end;
      padding: 8px 4px 6px 4px;
      max-width: 500px;
      margin: 0 auto;
    }

    /* Regular Nav Link */
    .mobile-link {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      text-decoration: none;
      color: var(--text-muted);
      gap: 2px;
      padding: 6px 8px;
      border-radius: 14px;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      position: relative;
      min-width: 56px;
      -webkit-tap-highlight-color: transparent;
    }

    .icon-wrap {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 36px;
      height: 36px;
      border-radius: 12px;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      position: relative;
    }

    .icon-wrap svg {
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }

    .mobile-link .label {
      font-size: 9px;
      font-weight: 600;
      letter-spacing: 0.3px;
      text-transform: uppercase;
      opacity: 0.7;
      transition: all 0.3s ease;
    }

    .active-dot {
      width: 4px;
      height: 4px;
      border-radius: 50%;
      background: var(--primary);
      position: absolute;
      bottom: 0;
      opacity: 0;
      transform: scale(0);
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }

    /* Active State */
    .mobile-link.active {
      color: var(--primary);
    }

    .mobile-link.active .icon-wrap {
      background: rgba(139, 92, 246, 0.15);
      box-shadow: 0 0 20px rgba(139, 92, 246, 0.2);
    }

    .mobile-link.active .icon-wrap svg {
      filter: drop-shadow(0 0 6px rgba(139, 92, 246, 0.5));
      transform: scale(1.1);
    }

    .mobile-link.active .label {
      opacity: 1;
      color: var(--primary);
      font-weight: 700;
    }

    .mobile-link.active .active-dot {
      opacity: 1;
      transform: scale(1);
      box-shadow: 0 0 8px rgba(139, 92, 246, 0.6);
    }

    /* Tap Effect */
    .mobile-link:active:not(.play-btn) {
      transform: scale(0.9);
    }

    .mobile-link:active:not(.play-btn) .icon-wrap {
      background: rgba(139, 92, 246, 0.1);
    }

    /* ============================================ */
    /* Center Play Button - Floating Design         */
    /* ============================================ */
    .play-btn {
      position: relative;
      bottom: 12px;
      align-self: flex-end;
      padding: 0;
      gap: 4px;
    }

    .play-btn-inner {
      width: 56px;
      height: 56px;
      border-radius: 50%;
      background: linear-gradient(135deg, #8b5cf6, #6d28d9, #7c3aed);
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;
      box-shadow:
        0 4px 20px rgba(139, 92, 246, 0.5),
        0 0 40px rgba(139, 92, 246, 0.2),
        inset 0 1px 0 rgba(255, 255, 255, 0.2);
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      position: relative;
      z-index: 2;
    }

    .play-btn-inner::before {
      content: '';
      position: absolute;
      inset: -3px;
      border-radius: 50%;
      background: linear-gradient(135deg, rgba(139, 92, 246, 0.6), rgba(109, 40, 217, 0.3));
      z-index: -1;
      opacity: 0;
      transition: opacity 0.3s ease;
    }

    .play-btn-inner::after {
      content: '';
      position: absolute;
      inset: -6px;
      border-radius: 50%;
      border: 2px solid rgba(139, 92, 246, 0.2);
      animation: play-pulse 2s ease-in-out infinite;
    }

    .play-btn-inner svg {
      margin-left: 2px;
      filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.3));
    }

    .play-label {
      font-size: 9px !important;
      font-weight: 700 !important;
      letter-spacing: 0.5px;
      text-transform: uppercase;
      color: var(--primary) !important;
      opacity: 1 !important;
    }

    /* Play Button Active State */
    .play-btn.active .play-btn-inner {
      background: linear-gradient(135deg, #7c3aed, #5b21b6, #6d28d9);
      box-shadow:
        0 4px 25px rgba(139, 92, 246, 0.7),
        0 0 60px rgba(139, 92, 246, 0.3),
        inset 0 1px 0 rgba(255, 255, 255, 0.3);
      transform: scale(1.05);
    }

    .play-btn.active .play-btn-inner::before {
      opacity: 1;
    }

    /* Play Button Hover / Tap */
    .play-btn:active .play-btn-inner {
      transform: scale(0.92);
      box-shadow:
        0 2px 15px rgba(139, 92, 246, 0.4),
        0 0 30px rgba(139, 92, 246, 0.15);
    }

    @keyframes play-pulse {
      0%, 100% {
        transform: scale(1);
        opacity: 0.5;
      }
      50% {
        transform: scale(1.15);
        opacity: 0;
      }
    }

    @keyframes float {
      0%, 100% { transform: translateY(0); }
      50% { transform: translateY(-5px); }
    }
  `]
})
export class MainLayoutComponent {
  constructor(public authService: AuthService) { }
}

