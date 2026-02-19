import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../services/admin.service';
import { UserProfile } from '../../../models/user.models';
import { ToastService } from '../../../services/toast.service';

@Component({
    selector: 'app-user-management',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './user-management.component.html',
    styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
    users: UserProfile[] = [];
    isLoading = false;

    constructor(
        private adminService: AdminService,
        private toastService: ToastService
    ) { }

    ngOnInit(): void {
        this.loadUsers();
    }

    loadUsers(): void {
        this.isLoading = true;
        this.adminService.getAllUsers().subscribe({
            next: (users) => {
                this.users = users;
                this.isLoading = false;
            },
            error: () => {
                this.toastService.error('Kullanıcılar yüklenirken bir hata oluştu.');
                this.isLoading = false;
            }
        });
    }

    toggleStatus(userId: string): void {
        this.adminService.toggleUserStatus(userId).subscribe({
            next: (success) => {
                if (success) {
                    const user = this.users.find(u => u.id === userId);
                    if (user) {
                        // We don't have isActive in UserProfile yet, might need to add it or refresh
                        this.loadUsers();
                    }
                    this.toastService.success('Kullanıcı durumu güncellendi.');
                }
            },
            error: () => this.toastService.error('İşlem başarısız oldu.')
        });
    }
}
