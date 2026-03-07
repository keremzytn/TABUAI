import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ToastService } from '../../../services/toast.service';
import { LoginRequest } from '../../../models/auth.models';
import { SocialAuthService, GoogleLoginProvider, FacebookLoginProvider, GoogleSigninButtonModule } from '@abacritt/angularx-social-login';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, GoogleSigninButtonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginData: LoginRequest = { username: '', password: '' };
  isLoading = false;
  socialLoading = false;
  error = '';

  constructor(
    private authService: AuthService,
    private socialAuthService: SocialAuthService,
    private router: Router,
    private toastService: ToastService
  ) {
    // Google veya Facebook girişi başarılı olduğunda bu state tetiklenir
    this.socialAuthService.authState.subscribe((user) => {
      if (user) {
        this.socialLoading = true;
        // Google idToken kullanırken, Facebook authToken kullanır.
        const token = user.provider === 'GOOGLE' ? user.idToken : user.authToken;
        this.handleSocialLogin(user.provider, token);
      }
    });
  }

  // Google butonu artık otomatik tetikleniyor, manual metod devre dışı bırakıldı.
  // Facebook hala manual metod ile çalışabilir.

  signInWithFB(): void {
    this.socialLoading = true;
    this.socialAuthService.signIn(FacebookLoginProvider.PROVIDER_ID).catch(err => {
      this.socialLoading = false;
      console.error(err);
      this.toastService.error('Facebook ile giriş yapılamadı.');
    });
  }

  private handleSocialLogin(provider: string, token: string) {
    this.authService.externalLogin(provider, token).subscribe({
      next: (user) => {
        this.socialLoading = false;
        this.toastService.success(`Hoş geldin, ${user.displayName || user.username}! 🎉`);
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.socialLoading = false;
        this.toastService.error('Sistem girişi başarısız oldu.');
        console.error(err);
      }
    });
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
        this.toastService.success(`Hoş geldin, ${user.displayName || user.username}! 🎉`);
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
