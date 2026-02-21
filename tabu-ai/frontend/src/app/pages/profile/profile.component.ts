import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UserService } from '../../services/user.service';
import { UserProfile, UserStatistic, GameHistory } from '../../models/user.models';
import { HttpClientModule } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [CommonModule, HttpClientModule, RouterModule],
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
    userId: string | null = null;
    profile: UserProfile | null = null;
    statistics: UserStatistic[] = [];
    gameHistory: GameHistory[] = [];
    isLoading = true;
    selectedGame: GameHistory | null = null;

    // Level thresholds
    private levelThresholds: Record<string, { min: number; max: number; next: string }> = {
        'Rookie': { min: 0, max: 300, next: 'Apprentice' },
        'Apprentice': { min: 300, max: 700, next: 'Skilled' },
        'Skilled': { min: 700, max: 1500, next: 'Expert' },
        'Expert': { min: 1500, max: 3000, next: 'Master' },
        'Master': { min: 3000, max: 5000, next: 'GrandMaster' },
        'GrandMaster': { min: 5000, max: 10000, next: 'GrandMaster' }
    };

    constructor(
        private userService: UserService,
        private authService: AuthService,
        private toastService: ToastService
    ) { }

    ngOnInit(): void {
        const user = this.authService.currentUserValue;
        if (user) {
            this.userId = user.id;
            this.loadData(user.id);
        } else {
            this.isLoading = false;
        }
    }

    loadData(userId: string) {
        this.isLoading = true;

        this.userService.getUserProfile(userId).subscribe({
            next: (data) => {
                this.profile = data;
                this.loadStatistics(userId);
                this.loadHistory(userId);
            },
            error: (err) => {
                console.error('Failed to load profile', err);
                this.toastService.error('Profil yüklenemedi.');
                this.isLoading = false;
            }
        });
    }

    loadStatistics(userId: string) {
        this.userService.getUserStatistics(userId).subscribe({
            next: (data) => this.statistics = data,
            error: (err) => console.error('Failed to load stats', err)
        });
    }

    loadHistory(userId: string) {
        this.userService.getUserGameHistory(userId).subscribe({
            next: (data) => {
                this.gameHistory = data;
                this.isLoading = false;
            },
            error: (err) => {
                console.error('Failed to load history', err);
                this.isLoading = false;
            }
        });
    }

    getLevelText(level: string): string {
        const map: Record<string, string> = {
            'Rookie': '🌱 Çaylak',
            'Apprentice': '⚡ Çırak',
            'Skilled': '💪 Becerikli',
            'Expert': '🎯 Uzman',
            'Master': '👑 Usta',
            'GrandMaster': '🏆 Büyük Usta'
        };
        return map[level] || level;
    }

    getNextLevelText(): string {
        if (!this.profile) return '';
        const threshold = this.levelThresholds[this.profile.level];
        if (!threshold || threshold.next === this.profile.level) return '🏆 Maksimum Seviye!';
        return this.getLevelText(threshold.next);
    }

    getNextLevelScore(): number {
        if (!this.profile) return 300;
        const threshold = this.levelThresholds[this.profile.level];
        return threshold?.max || 10000;
    }

    getLevelProgress(): number {
        if (!this.profile) return 0;
        const threshold = this.levelThresholds[this.profile.level];
        if (!threshold) return 0;

        const progress = ((this.profile.totalScore - threshold.min) / (threshold.max - threshold.min)) * 100;
        return Math.min(100, Math.max(2, progress));
    }

    getBadgeEmoji(badgeName: string): string {
        const emojiMap: Record<string, string> = {
            'İlk Adım': '👣',
            'Çaylak Anlatıcı': '🌱',
            'Hızlı Eller': '⚡',
            'Kelime Ustası': '📝',
            'Prompt Sihirbazı': '🧙',
            'Tabu Avcısı': '🎯',
            'Seri Katil': '🔥',
            'Hız Şeytanı': '💨',
            'Mükemmeliyetçi': '💎',
            'Kategori Kralı': '👑'
        };
        return emojiMap[badgeName] || '🏅';
    }

    openGamePopup(game: GameHistory): void {
        this.selectedGame = game;
    }

    closeGamePopup(): void {
        this.selectedGame = null;
    }

    formatTimeSpent(timeSpent: string): string {
        if (!timeSpent) return '—';
        // TimeSpan format: "HH:mm:ss" or "d.HH:mm:ss"
        const parts = timeSpent.split(':');
        if (parts.length >= 3) {
            const hours = parseInt(parts[0], 10);
            const minutes = parseInt(parts[1], 10);
            const seconds = parseInt(parts[2], 10);
            if (hours > 0) return `${hours}sa ${minutes}dk ${seconds}sn`;
            if (minutes > 0) return `${minutes}dk ${seconds}sn`;
            return `${seconds}sn`;
        }
        return timeSpent;
    }

    getModeName(mode: number): string {
        const modeMap: Record<number, string> = {
            0: 'Klasik',
            1: 'Zamana Karşı',
            2: 'Zorluk Modu'
        };
        return modeMap[mode] ?? 'Klasik';
    }
}
