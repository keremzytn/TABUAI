import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService, UserDetail } from '../../services/admin.service';
import { ToastService } from '../../../services/toast.service';

@Component({
    selector: 'app-user-detail',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './user-detail.component.html',
    styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent implements OnInit {
    user: UserDetail | null = null;
    isLoading = false;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private adminService: AdminService,
        private toastService: ToastService
    ) { }

    ngOnInit(): void {
        const userId = this.route.snapshot.paramMap.get('id');
        if (userId) {
            this.loadUser(userId);
        }
    }

    loadUser(userId: string): void {
        this.isLoading = true;
        this.adminService.getUserDetail(userId).subscribe({
            next: (user) => {
                this.user = user;
                this.isLoading = false;
            },
            error: () => {
                this.toastService.error('Kullanıcı detayları yüklenemedi.');
                this.isLoading = false;
            }
        });
    }

    goBack(): void {
        this.router.navigate(['/admin/users']);
    }

    formatDate(dateStr: string): string {
        return new Date(dateStr).toLocaleDateString('tr-TR', {
            year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit'
        });
    }
}
