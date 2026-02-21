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
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
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