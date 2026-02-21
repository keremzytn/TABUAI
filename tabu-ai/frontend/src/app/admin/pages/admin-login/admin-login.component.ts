import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
    selector: 'app-admin-login',
    standalone: true,
    imports: [CommonModule, FormsModule, RouterLink],
    template: `
    <div class="admin-login-page">
      <div class="login-card">
        <div class="login-header">
          <span class="lock-icon">🔒</span>
          <h1>Admin Panele Giriş</h1>
          <p>Sadece yetkili personel içindir.</p>
        </div>

        <form (ngSubmit)="onSubmit()" #loginForm="ngForm" class="login-form">
          <div class="form-group">
            <label for="username">Kullanıcı Adı</label>
            <input 
              type="text" 
              id="username" 
              name="username" 
              [(ngModel)]="loginData.username" 
              required 
              placeholder="admin"
            >
          </div>

          <div class="form-group">
            <label for="password">Şifre</label>
            <input 
              type="password" 
              id="password" 
              name="password" 
              [(ngModel)]="loginData.password" 
              required
              placeholder="••••••••"
            >
          </div>

          <div class="form-error" *ngIf="errorMessage">
            {{errorMessage}}
          </div>

          <button type="submit" [disabled]="loginForm.invalid || isLoading" class="login-btn">
            <span *ngIf="!isLoading">Giriş Yap</span>
            <span *ngIf="isLoading" class="loader"></span>
          </button>
        </form>

        <div class="login-footer">
          <a routerLink="/" class="back-link">🏠 Ana Sayfaya Dön</a>
        </div>
      </div>
    </div>
  `,
    styles: [`
    .admin-login-page {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: #0f172a;
      padding: 20px;
    }

    .login-card {
      width: 100%;
      max-width: 400px;
      background: #1e293b;
      padding: 40px;
      border-radius: 16px;
      box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
      border: 1px solid rgba(255, 255, 255, 0.05);
    }

    .login-header {
      text-align: center;
      margin-bottom: 32px;
    }

    .lock-icon {
      font-size: 48px;
      display: block;
      margin-bottom: 16px;
    }

    .login-header h1 {
      font-size: 24px;
      font-weight: 700;
      color: white;
      margin: 0 0 8px 0;
    }

    .login-header p {
      color: #94a3b8;
      font-size: 14px;
    }

    .login-form {
      display: flex;
      flex-direction: column;
      gap: 20px;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .form-group label {
      font-size: 14px;
      font-weight: 600;
      color: #cbd5e1;
    }

    .form-group input {
      background: #0f172a;
      border: 1px solid #334155;
      padding: 12px 16px;
      border-radius: 8px;
      color: white;
      font-size: 16px;
      transition: all 0.2s;
    }

    .form-group input:focus {
      outline: none;
      border-color: #38bdf8;
      box-shadow: 0 0 0 2px rgba(56, 189, 248, 0.2);
    }

    .form-error {
      background: rgba(239, 68, 68, 0.1);
      color: #ef4444;
      padding: 10px;
      border-radius: 8px;
      font-size: 14px;
      text-align: center;
    }

    .login-btn {
      background: #38bdf8;
      color: #0f172a;
      border: none;
      padding: 14px;
      border-radius: 8px;
      font-size: 16px;
      font-weight: 700;
      cursor: pointer;
      transition: all 0.2s;
      margin-top: 8px;
    }

    .login-btn:hover:not(:disabled) {
      background: #0ea5e9;
      transform: translateY(-1px);
    }

    .login-btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .login-footer {
      margin-top: 32px;
      text-align: center;
    }

    .back-link {
      color: #94a3b8;
      text-decoration: none;
      font-size: 14px;
      transition: color 0.2s;
    }

    .back-link:hover {
      color: white;
    }

    .loader {
      width: 20px;
      height: 20px;
      border: 3px solid rgba(15, 23, 42, 0.3);
      border-radius: 50%;
      border-top-color: #0f172a;
      display: inline-block;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class AdminLoginComponent {
    loginData = {
        username: '',
        password: ''
    };
    isLoading = false;
    errorMessage = '';

    constructor(private authService: AuthService, private router: Router) { }

    onSubmit() {
        this.isLoading = true;
        this.errorMessage = '';

        this.authService.login(this.loginData).subscribe({
            next: (user) => {
                if (user.role === 'Admin') {
                    this.router.navigate(['/admin']);
                } else {
                    this.errorMessage = 'Yalnızca yönetici girişi yapılabilir.';
                    this.authService.logout();
                }
                this.isLoading = false;
            },
            error: (err) => {
                this.errorMessage = 'Giriş başarısız. Lütfen bilgilerinizi kontrol edin.';
                this.isLoading = false;
            }
        });
    }
}
