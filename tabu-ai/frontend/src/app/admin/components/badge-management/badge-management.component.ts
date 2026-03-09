import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService, BadgeAdmin } from '../../services/admin.service';
import { UserProfile } from '../../../models/user.models';
import { ToastService } from '../../../services/toast.service';

@Component({
    selector: 'app-badge-management',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './badge-management.component.html',
    styleUrls: ['./badge-management.component.css']
})
export class BadgeManagementComponent implements OnInit {
    badges: BadgeAdmin[] = [];
    isLoading = false;
    isModalOpen = false;
    isEditing = false;

    currentBadge = this.getEmptyBadge();

    isAssignModalOpen = false;
    assignBadgeId = '';
    assignBadgeName = '';
    assignUserId = '';
    assignUserSearch = '';
    users: UserProfile[] = [];
    filteredUsers: UserProfile[] = [];
    isAssigning = false;

    badgeTypes = [
        { value: 1, label: 'Oyun Kazanma' },
        { value: 2, label: 'Ardışık Kazanım' },
        { value: 3, label: 'Mükemmel Prompt' },
        { value: 4, label: 'Hızlı Tamamlama' },
        { value: 5, label: 'Tabu Kaçınma' },
        { value: 6, label: 'Seri' },
        { value: 7, label: 'Kategori Uzmanı' },
        { value: 8, label: 'Seviye Atlama' },
        { value: 9, label: 'Yüksek Skor' },
        { value: 10, label: 'Özveri' }
    ];

    constructor(
        private adminService: AdminService,
        private toastService: ToastService
    ) { }

    ngOnInit(): void {
        this.loadBadges();
        this.loadUsers();
    }

    loadUsers(): void {
        this.adminService.getAllUsers().subscribe({
            next: (users) => {
                this.users = users;
                this.filteredUsers = users;
            },
            error: () => { }
        });
    }

    loadBadges(): void {
        this.isLoading = true;
        this.adminService.getAllBadges().subscribe({
            next: (badges) => {
                this.badges = badges;
                this.isLoading = false;
            },
            error: () => {
                this.toastService.error('Rozetler yüklenirken hata oluştu.');
                this.isLoading = false;
            }
        });
    }

    openModal(badge?: BadgeAdmin): void {
        if (badge) {
            this.isEditing = true;
            this.currentBadge = {
                id: badge.id,
                name: badge.name,
                description: badge.description,
                iconUrl: badge.iconUrl,
                type: this.badgeTypes.find(t => t.label === badge.type)?.value || 1,
                requiredValue: badge.requiredValue,
                isActive: badge.isActive
            };
        } else {
            this.isEditing = false;
            this.currentBadge = this.getEmptyBadge();
        }
        this.isModalOpen = true;
    }

    closeModal(): void {
        this.isModalOpen = false;
    }

    saveBadge(): void {
        if (this.isEditing) {
            this.adminService.updateBadge(this.currentBadge as any).subscribe({
                next: () => {
                    this.toastService.success('Rozet güncellendi.');
                    this.loadBadges();
                    this.closeModal();
                },
                error: () => this.toastService.error('Güncelleme başarısız.')
            });
        } else {
            this.adminService.createBadge(this.currentBadge).subscribe({
                next: () => {
                    this.toastService.success('Yeni rozet oluşturuldu.');
                    this.loadBadges();
                    this.closeModal();
                },
                error: () => this.toastService.error('Oluşturma başarısız.')
            });
        }
    }

    deleteBadge(id: string): void {
        if (confirm('Bu rozeti silmek istediğinize emin misiniz?')) {
            this.adminService.deleteBadge(id).subscribe({
                next: () => {
                    this.toastService.success('Rozet silindi.');
                    this.loadBadges();
                },
                error: () => this.toastService.error('Silme işlemi başarısız.')
            });
        }
    }

    openAssignModal(badge: BadgeAdmin): void {
        this.assignBadgeId = badge.id;
        this.assignBadgeName = badge.name;
        this.assignUserId = '';
        this.assignUserSearch = '';
        this.filteredUsers = this.users;
        this.isAssignModalOpen = true;
    }

    closeAssignModal(): void {
        this.isAssignModalOpen = false;
    }

    onUserSearch(): void {
        const q = this.assignUserSearch.toLowerCase();
        this.filteredUsers = this.users.filter(u =>
            u.username.toLowerCase().includes(q) ||
            (u.displayName || '').toLowerCase().includes(q)
        );
    }

    selectUser(userId: string): void {
        this.assignUserId = userId;
    }

    confirmAssign(): void {
        if (!this.assignUserId) {
            this.toastService.error('Lütfen bir kullanıcı seçin.');
            return;
        }
        this.isAssigning = true;
        this.adminService.assignBadge(this.assignBadgeId, this.assignUserId).subscribe({
            next: () => {
                this.toastService.success(`"${this.assignBadgeName}" rozeti kullanıcıya verildi.`);
                this.loadBadges();
                this.closeAssignModal();
                this.isAssigning = false;
            },
            error: () => {
                this.toastService.error('Rozet verilemedi. Kullanıcı bu rozete zaten sahip olabilir.');
                this.isAssigning = false;
            }
        });
    }

    getTypeName(type: string): string {
        return type;
    }

    private getEmptyBadge() {
        return {
            id: '',
            name: '',
            description: '',
            iconUrl: '',
            type: 1,
            requiredValue: 1,
            isActive: true
        };
    }
}
