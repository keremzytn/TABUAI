import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ToastService } from '../../../services/toast.service';
import { LoginRequest } from '../../../models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginData: LoginRequest = { username: '', password: '' };
  isLoading = false;
  error = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private toastService: ToastService
  ) {}

  loginWithGoogle() {
    this.toastService.info('Google ile giriş yapılıyor...');
  }

  loginWithMicrosoft() {
    const clientId = ''; // Azure AD App Registration Client ID
    const redirectUri = encodeURIComponent(window.location.origin + '/login');
    const scope = encodeURIComponent('openid profile email User.Read');
    const msLoginUrl = `https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=${clientId}&response_type=token&redirect_uri=${redirectUri}&scope=${scope}&response_mode=fragment`;

    if (!clientId) {
      this.toastService.info('Microsoft SSO yapılandırması bekleniyor.');
      return;
    }
    window.location.href = msLoginUrl;
  }

  onSubmit() {
    if (!this.loginData.username || !this.loginData.password) {
      this.error = 'Lütfen kullanıcı adı ve şifre giriniz.';
      this.toastService.warning('Lütfen tüm alanları doldurun.');
      return;
    }

    this.isLoading = true;
    this.error = '';

    this.authService.login(this.loginData).subscribe({
      next: (user) => {
        this.toastService.success(`Hoş geldin, ${user.displayName || user.username}!`);
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.error = 'Giriş başarısız. Lütfen bilgilerinizi kontrol edin.';
        this.toastService.error('Giriş başarısız. Bilgilerinizi kontrol edin.');
        this.isLoading = false;
        console.error(err);
      }
    });
  }
}
