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

    canActivate(route: import('@angular/router').ActivatedRouteSnapshot): boolean | UrlTree {
        const user = this.authService.currentUserValue;
        if (user) {
            // Check if route has restricted roles
            const expectedRole = route.data['role'];
            if (expectedRole && user.role !== expectedRole) {
                this.toastService.error('Bu sayfaya erişim yetkiniz yok.');
                return this.router.createUrlTree(['/home']);
            }
            return true;
        }

        this.toastService.warning('Bu sayfaya erişmek için giriş yapmalısınız.');
        return this.router.createUrlTree(['/login']);
    }
}
