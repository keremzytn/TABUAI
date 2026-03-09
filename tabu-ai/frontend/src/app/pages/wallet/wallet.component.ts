import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EconomyService } from '../../services/economy.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { CoinBalance, ShopItem, InventoryResponse } from '../../models/game.models';

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
    inventory: InventoryResponse | null = null;
    isLoading = true;
    purchasingItem: string | null = null;
    activeTab: 'wallet' | 'shop' | 'inventory' | 'history' = 'wallet';
    shopCategory: string = 'all';

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
        this.economyService.getInventory().subscribe({
            next: (inv) => this.inventory = inv
        });
    }

    purchaseItem(item: ShopItem) {
        if (!this.balance || this.balance.balance < item.price || item.isOwned) return;
        this.purchasingItem = item.id;

        this.economyService.purchaseItem(item.id).subscribe({
            next: (result) => {
                this.purchasingItem = null;
                if (result.success) {
                    this.toastService.success(result.message || 'Satın alındı!');
                    this.loadData();
                } else {
                    this.toastService.error(result.errorMessage || 'Satın alma başarısız');
                }
            },
            error: () => {
                this.purchasingItem = null;
                this.toastService.error('Bir hata oluştu');
            }
        });
    }

    equipItem(type: string, itemId: string | null) {
        this.economyService.equipItem(type, itemId).subscribe({
            next: () => {
                this.toastService.success('Seçim güncellendi!');
                this.loadData();
            },
            error: () => this.toastService.error('Hata oluştu')
        });
    }

    get filteredShopItems(): ShopItem[] {
        if (this.shopCategory === 'all') return this.shopItems;
        return this.shopItems.filter(i => i.category === this.shopCategory);
    }

    get shopCategories(): string[] {
        return ['all', ...new Set(this.shopItems.map(i => i.category))];
    }

    getCategoryLabel(cat: string): string {
        const labels: Record<string, string> = {
            'all': 'Tümü',
            'Gameplay': 'Oyun',
            'Streak': 'Seri',
            'Boost': 'Boost',
            'Avatar': 'Avatar',
            'CardDesign': 'Kart'
        };
        return labels[cat] || cat;
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
            'CardDesignPurchase': '🃏',
            'LevelUpBonus': '⬆️',
            'ShopPurchase': '🛒',
            'StreakMilestoneReward': '🏆',
            'StreakShieldUsed': '🛡️'
        };
        return icons[type] || '💰';
    }

    getNextStreakMilestone(): number {
        if (!this.balance) return 5;
        const milestones = [5, 10, 15, 20, 25, 30, 50, 100];
        return milestones.find(m => m > this.balance!.currentStreak) || 100;
    }

    getStreakProgress(): number {
        if (!this.balance) return 0;
        const next = this.getNextStreakMilestone();
        const milestones = [0, 5, 10, 15, 20, 25, 30, 50, 100];
        const prev = milestones.filter(m => m < next).pop() || 0;
        return Math.min(100, ((this.balance.currentStreak - prev) / (next - prev)) * 100);
    }

    isItemEquipped(itemId: string): boolean {
        if (!this.inventory) return false;
        return this.inventory.selectedAvatar === itemId || this.inventory.selectedCardDesign === itemId;
    }

    switchTab(tab: 'wallet' | 'shop' | 'inventory' | 'history') {
        this.activeTab = tab;
    }
}
