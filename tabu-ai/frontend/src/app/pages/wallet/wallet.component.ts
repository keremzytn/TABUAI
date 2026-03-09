import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EconomyService } from '../../services/economy.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { CoinBalance, ShopItem } from '../../models/game.models';

@Component({
    selector: 'app-wallet',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './wallet.component.html',
    styleUrls: ['./wallet.component.scss']
})
export class WalletComponent implements OnInit {
    balance: CoinBalance | null = null;
    shopItems: ShopItem[] = [];
    isLoading = true;
    activeTab: 'wallet' | 'shop' | 'history' = 'wallet';

    constructor(
        private economyService: EconomyService,
        private authService: AuthService,
        private toastService: ToastService
    ) {}

    ngOnInit(): void {
        if (this.authService.currentUserValue) {
            this.loadData();
        } else {
            this.isLoading = false;
        }
    }

    loadData() {
        this.isLoading = true;
        this.economyService.getBalance().subscribe({
            next: (data) => {
                this.balance = data;
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
                this.toastService.error('Cüzdan yüklenemedi.');
            }
        });
        this.economyService.getShopItems().subscribe({
            next: (items) => this.shopItems = items
        });
    }

    getStreakEmoji(): string {
        if (!this.balance) return '🔥';
        const s = this.balance.currentStreak;
        if (s >= 30) return '🌋';
        if (s >= 14) return '💥';
        if (s >= 7) return '🔥';
        if (s >= 3) return '✨';
        return '⚡';
    }

    getStreakMessage(): string {
        if (!this.balance) return '';
        const s = this.balance.currentStreak;
        if (s >= 30) return 'Efsanevi seri!';
        if (s >= 14) return 'Durdurulamaz!';
        if (s >= 7) return 'Harika gidiyorsun!';
        if (s >= 3) return 'Seri başladı!';
        if (s >= 1) return 'İlk adım!';
        return 'Oyna ve seri başlat!';
    }

    getTransactionIcon(type: string): string {
        const icons: Record<string, string> = {
            'GameWin': '🎮',
            'StreakBonus': '🔥',
            'DailyChallengeBonus': '📅',
            'HintPurchase': '💡',
            'AvatarPurchase': '🎨',
            'LevelUpBonus': '⬆️'
        };
        return icons[type] || '💰';
    }

    switchTab(tab: 'wallet' | 'shop' | 'history') {
        this.activeTab = tab;
    }
}
