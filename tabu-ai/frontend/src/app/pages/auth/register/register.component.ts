import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { ToastService } from '../../../services/toast.service';
import { RegisterRequest } from '../../../models/auth.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss' // Intentionally pointing to same SCSS or separate one. 
  // Wait, I should use its own SCSS. I'll copy the styles or import them.
})
export class RegisterComponent {
  registerData: RegisterRequest = { username: '', email: '', password: '', displayName: '' };
  isLoading = false;
  error = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private toastService: ToastService
  ) { }

  onSubmit() {
    if (!this.registerData.username || !this.registerData.email || !this.registerData.password) {
      this.error = 'Lütfen tüm zorunlu alanları doldurun.';
      this.toastService.warning('Lütfen tüm zorunlu alanları doldurun.');
      return;
    }

    if (this.registerData.password.length < 6) {
      this.error = 'Şifre en az 6 karakter olmalıdır.';
      this.toastService.warning('Şifre en az 6 karakter olmalıdır.');
      return;
    }

    if (!this.registerData.email.includes('@')) {
      this.error = 'Geçerli bir e-posta adresi giriniz.';
      this.toastService.warning('Geçerli bir e-posta adresi giriniz.');
      return;
    }

    this.isLoading = true;
    this.error = '';

    this.authService.register(this.registerData).subscribe({
      next: (user) => {
        this.toastService.success(`Hoş geldin, ${user.displayName || user.username}! 🌟 Kayıt Başarılı!`);
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.error = 'Kayıt başarısız. Lütfen bilgilerinizi kontrol edin.';
        this.toastService.error('Kayıt başarısız. Bilgilerinizi kontrol edin.');
        this.isLoading = false;
        console.error(err);
      }
    });
  }
}
