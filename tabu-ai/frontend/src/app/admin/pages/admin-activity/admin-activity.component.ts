import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, GameSessionAdmin, ActivityLogAdmin, PagedResult } from '../../services/admin.service';
import { ToastService } from '../../../services/toast.service';

type ActiveTab = 'games' | 'logs';

@Component({
    selector: 'app-admin-activity',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './admin-activity.component.html',
    styleUrls: ['./admin-activity.component.css']
})
export class AdminActivityComponent implements OnInit {
    activeTab: ActiveTab = 'games';

    gameSessions: GameSessionAdmin[] = [];
    gamesTotal = 0;
    gamesPage = 1;
    gamesTotalPages = 1;
    gamesLoading = false;

    activityLogs: ActivityLogAdmin[] = [];
    logsTotal = 0;
    logsPage = 1;
    logsTotalPages = 1;
    logsLoading = false;

    constructor(
        private adminService: AdminService,
        private toastService: ToastService
    ) { }

    ngOnInit(): void {
        this.loadGameSessions();
        this.loadActivityLogs();
    }

    loadGameSessions(page = 1): void {
        this.gamesLoading = true;
        this.gamesPage = page;
        this.adminService.getGameSessions(page, 50).subscribe({
            next: (result: PagedResult<GameSessionAdmin>) => {
                this.gameSessions = result.items;
                this.gamesTotal = result.totalCount;
                this.gamesTotalPages = result.totalPages;
                this.gamesLoading = false;
            },
            error: () => {
                this.toastService.error('Oyun verileri yüklenemedi.');
                this.gamesLoading = false;
            }
        });
    }

    loadActivityLogs(page = 1): void {
        this.logsLoading = true;
        this.logsPage = page;
        this.adminService.getActivityLogs(page, 100).subscribe({
            next: (result: PagedResult<ActivityLogAdmin>) => {
                this.activityLogs = result.items;
                this.logsTotal = result.totalCount;
                this.logsTotalPages = result.totalPages;
                this.logsLoading = false;
            },
            error: () => {
                this.toastService.error('Aktivite logları yüklenemedi.');
                this.logsLoading = false;
            }
        });
    }

    setTab(tab: ActiveTab): void {
        this.activeTab = tab;
    }

    formatDate(dateStr: string): string {
        return new Date(dateStr).toLocaleDateString('tr-TR', {
            year: 'numeric', month: '2-digit', day: '2-digit',
            hour: '2-digit', minute: '2-digit'
        });
    }
}
