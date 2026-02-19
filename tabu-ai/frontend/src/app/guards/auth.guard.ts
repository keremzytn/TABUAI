import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {
    constructor(
        private authService: AuthService,
        private router: Router,
        private toastService: ToastService
    ) { }

    canActivate(): boolean | UrlTree {
        if (this.authService.currentUserValue) {
            return true;
        }

        this.toastService.warning('Bu sayfaya erişmek için giriş yapmalısınız.');
        return this.router.createUrlTree(['/login']);
    }
}
