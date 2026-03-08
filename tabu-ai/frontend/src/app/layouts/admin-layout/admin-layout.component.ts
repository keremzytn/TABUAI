import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterLink, RouterLinkActive],
  template: `
    <div class="admin-layout">
      <!-- Admin Sidebar -->
      <aside class="admin-sidebar">
        <div class="sidebar-header">
          <span class="admin-logo">🛡️</span>
          <h2>ADMIN<span class="panel-tag">PANEL</span></h2>
        </div>
        
        <nav class="sidebar-nav">
          <a routerLink="/admin/users" routerLinkActive="active" class="sidebar-link">
            <span class="link-icon">👥</span>
            Kullanıcı Yönetimi
          </a>
          <a routerLink="/admin/words" routerLinkActive="active" class="sidebar-link">
            <span class="link-icon">📝</span>
            Kelime Yönetimi
          </a>
          <div class="nav-divider"></div>
          <a routerLink="/" class="sidebar-link return-link">
            <span class="link-icon">🏠</span>
            Siteye Dön
          </a>
        </nav>

        <div class="sidebar-footer">
          <button (click)="logout()" class="logout-btn">
            <span class="icon">🚪</span> Çıkış Yap
          </button>
        </div>
      </aside>

      <!-- Main Admin Content -->
      <main class="admin-main">
        <header class="admin-top-bar">
          <div class="current-page">
            <span class="breadcrumb">Admin / {{currentPage}}</span>
          </div>
          <div class="admin-user-info" *ngIf="authService.currentUser | async as user">
            <span class="user-name">{{user.displayName || user.username}}</span>
            <div class="user-avatar">{{(user.displayName || user.username)[0] || 'A'}}</div>
          </div>
        </header>
        
        <div class="admin-content-wrapper">
          <router-outlet></router-outlet>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .admin-layout {
      display: flex;
      min-height: 100vh;
      background: #0f172a;
      color: #e2e8f0;
    }

    /* Sidebar */
    .admin-sidebar {
      width: 280px;
      background: #1e293b;
      border-right: 1px solid rgba(255, 255, 255, 0.05);
      display: flex;
      flex-direction: column;
      position: fixed;
      height: 100vh;
      z-index: 100;
    }

    .sidebar-header {
      padding: 32px 24px;
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .admin-logo {
      font-size: 32px;
    }

    .sidebar-header h2 {
      font-size: 20px;
      font-weight: 800;
      letter-spacing: 1px;
      margin: 0;
    }

    .panel-tag {
      color: #38bdf8;
      font-weight: 400;
      margin-left: 4px;
    }

    .sidebar-nav {
      flex: 1;
      padding: 0 16px;
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .sidebar-link {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 16px;
      border-radius: 12px;
      color: #94a3b8;
      text-decoration: none;
      font-weight: 500;
      transition: all 0.2s;
    }

    .sidebar-link:hover {
      background: rgba(255, 255, 255, 0.05);
      color: white;
    }

    .sidebar-link.active {
      background: rgba(56, 189, 248, 0.1);
      color: #38bdf8;
    }

    .nav-divider {
      height: 1px;
      background: rgba(255, 255, 255, 0.05);
      margin: 16px 0;
    }

    .return-link {
        font-size: 14px;
    }

    .sidebar-footer {
      padding: 24px;
      border-top: 1px solid rgba(255, 255, 255, 0.05);
    }

    .logout-btn {
      width: 100%;
      background: rgba(239, 68, 68, 0.1);
      color: #ef4444;
      border: 1px solid rgba(239, 68, 68, 0.2);
      padding: 10px;
      border-radius: 8px;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      font-weight: 600;
      transition: all 0.2s;
    }

    .logout-btn:hover {
      background: #ef4444;
      color: white;
    }

    /* Main Content */
    .admin-main {
      flex: 1;
      margin-left: 280px;
      display: flex;
      flex-direction: column;
    }

    .admin-top-bar {
      height: 80px;
      padding: 0 40px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: #1e293b;
      border-bottom: 1px solid rgba(255, 255, 255, 0.05);
      position: sticky;
      top: 0;
      z-index: 90;
    }

    .breadcrumb {
      color: #94a3b8;
      font-size: 14px;
      font-weight: 500;
    }

    .admin-user-info {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .user-name {
      font-weight: 600;
      font-size: 14px;
    }

    .user-avatar {
      width: 40px;
      height: 40px;
      background: #38bdf8;
      color: white;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 700;
    }

    .admin-content-wrapper {
      padding: 40px;
      flex: 1;
    }

    @media (max-width: 1024px) {
        .admin-sidebar {
            width: 80px;
        }
        .admin-sidebar h2, .sidebar-link span:not(.link-icon), .logout-btn span:not(.icon) {
            display: none;
        }
        .admin-main {
            margin-left: 80px;
        }
        .sidebar-header {
            justify-content: center;
            padding: 24px 0;
        }
        .sidebar-link {
            justify-content: center;
            padding: 16px;
        }
        .sidebar-footer {
            padding: 16px;
        }
    }
  `]
})
export class AdminLayoutComponent {
  constructor(public authService: AuthService, private router: Router) { }

  get currentPage(): string {
    const url = this.router.url;
    if (url.includes('users')) return 'Kullanıcı Yönetimi';
    if (url.includes('words')) return 'Kelime Yönetimi';
    return 'Dashboard';
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/admin/login']);
  }
}
