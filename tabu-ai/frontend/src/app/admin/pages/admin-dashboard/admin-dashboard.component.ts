import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, DashboardStats } from '../../services/admin.service';
import { ToastService } from '../../../services/toast.service';

@Component({
    selector: 'app-admin-dashboard',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './admin-dashboard.component.html',
    styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
    stats: DashboardStats | null = null;
    isLoading = false;

    constructor(
        private adminService: AdminService,
        private toastService: ToastService
    ) { }

    ngOnInit(): void {
        this.loadStats();
    }

    loadStats(): void {
        this.isLoading = true;
        this.adminService.getDashboardStats().subscribe({
            next: (stats) => {
                this.stats = stats;
                this.isLoading = false;
            },
            error: () => {
                this.toastService.error('Dashboard verileri yüklenirken hata oluştu.');
                this.isLoading = false;
            }
        });
    }

    getMaxRegistration(): number {
        if (!this.stats) return 1;
        return Math.max(...this.stats.last7DaysRegistrations.map(r => r.count), 1);
    }

    getBarHeight(count: number): number {
        return (count / this.getMaxRegistration()) * 100;
    }

    formatDate(dateStr: string): string {
        const date = new Date(dateStr);
        return date.toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit' });
    }
}
