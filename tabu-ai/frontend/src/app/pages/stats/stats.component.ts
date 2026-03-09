import { Component, OnInit, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { PromptAnalysisChart, StyleAnalysis, BadgeGallery, BadgeShowcase } from '../../models/user.models';

@Component({
    selector: 'app-stats',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './stats.component.html',
    styleUrls: ['./stats.component.scss']
})
export class StatsComponent implements OnInit, AfterViewInit {
    @ViewChild('chartCanvas') chartCanvas!: ElementRef<HTMLCanvasElement>;

    userId: string | null = null;
    activeTab: 'chart' | 'style' | 'gallery' = 'chart';
    isLoading = true;

    chartData: PromptAnalysisChart | null = null;
    chartDays = 30;
    chartMetric: 'successRate' | 'averagePromptLength' | 'uniqueWordsUsed' | 'averageScore' = 'successRate';

    styleAnalysis: StyleAnalysis | null = null;

    badgeGallery: BadgeGallery | null = null;
    galleryFilter: 'all' | 'earned' | 'locked' = 'all';

    constructor(
        private userService: UserService,
        private authService: AuthService,
        private toastService: ToastService
    ) {}

    ngOnInit(): void {
        const user = this.authService.currentUserValue;
        if (user) {
            this.userId = user.id;
            this.loadAllData(user.id);
        } else {
            this.isLoading = false;
        }
    }

    ngAfterViewInit(): void {
        setTimeout(() => this.drawChart(), 500);
    }

    loadAllData(userId: string) {
        this.isLoading = true;
        this.loadChart(userId);
        this.loadStyle(userId);
        this.loadGallery(userId);
    }

    loadChart(userId: string) {
        this.userService.getPromptAnalysis(userId, this.chartDays).subscribe({
            next: (data) => {
                this.chartData = data;
                this.isLoading = false;
                setTimeout(() => this.drawChart(), 100);
            },
            error: () => {
                this.isLoading = false;
                this.toastService.error('Grafik verisi yüklenemedi.');
            }
        });
    }

    loadStyle(userId: string) {
        this.userService.getStyleAnalysis(userId).subscribe({
            next: (data) => this.styleAnalysis = data,
            error: () => this.toastService.error('Stil analizi yüklenemedi.')
        });
    }

    loadGallery(userId: string) {
        this.userService.getBadgeGallery(userId).subscribe({
            next: (data) => this.badgeGallery = data,
            error: () => this.toastService.error('Rozet galerisi yüklenemedi.')
        });
    }

    switchTab(tab: 'chart' | 'style' | 'gallery') {
        this.activeTab = tab;
        if (tab === 'chart') {
            setTimeout(() => this.drawChart(), 100);
        }
    }

    changeChartDays(days: number) {
        this.chartDays = days;
        if (this.userId) this.loadChart(this.userId);
    }

    changeMetric(metric: 'successRate' | 'averagePromptLength' | 'uniqueWordsUsed' | 'averageScore') {
        this.chartMetric = metric;
        this.drawChart();
    }

    getMetricLabel(): string {
        const labels: Record<string, string> = {
            'successRate': 'Başarı Oranı (%)',
            'averagePromptLength': 'Ort. Prompt Uzunluğu',
            'uniqueWordsUsed': 'Benzersiz Kelime',
            'averageScore': 'Ort. Skor'
        };
        return labels[this.chartMetric] || '';
    }

    getTotalGames(): number {
        if (!this.chartData) return 0;
        return this.chartData.dataPoints.reduce((sum, d) => sum + d.gamesPlayed, 0);
    }

    getAvgSuccess(): number {
        if (!this.chartData || this.chartData.dataPoints.length === 0) return 0;
        return this.chartData.dataPoints.reduce((sum, d) => sum + d.successRate, 0) / this.chartData.dataPoints.length;
    }

    drawChart() {
        if (!this.chartCanvas || !this.chartData || this.chartData.dataPoints.length === 0) return;

        const canvas = this.chartCanvas.nativeElement;
        const ctx = canvas.getContext('2d');
        if (!ctx) return;

        const dpr = window.devicePixelRatio || 1;
        const rect = canvas.getBoundingClientRect();
        canvas.width = rect.width * dpr;
        canvas.height = rect.height * dpr;
        ctx.scale(dpr, dpr);

        const w = rect.width;
        const h = rect.height;
        const padding = { top: 20, right: 20, bottom: 40, left: 50 };
        const chartW = w - padding.left - padding.right;
        const chartH = h - padding.top - padding.bottom;

        ctx.clearRect(0, 0, w, h);

        const data = this.chartData.dataPoints;
        const values = data.map(d => d[this.chartMetric] as number);
        const maxVal = Math.max(...values, 1);
        const minVal = Math.min(...values, 0);
        const range = maxVal - minVal || 1;

        ctx.strokeStyle = 'rgba(255,255,255,0.06)';
        ctx.lineWidth = 1;
        for (let i = 0; i <= 4; i++) {
            const y = padding.top + (chartH / 4) * i;
            ctx.beginPath();
            ctx.moveTo(padding.left, y);
            ctx.lineTo(w - padding.right, y);
            ctx.stroke();

            const val = maxVal - (range / 4) * i;
            ctx.fillStyle = 'rgba(255,255,255,0.3)';
            ctx.font = '10px JetBrains Mono, monospace';
            ctx.textAlign = 'right';
            ctx.fillText(val.toFixed(this.chartMetric === 'successRate' ? 0 : 1), padding.left - 8, y + 4);
        }

        ctx.fillStyle = 'rgba(255,255,255,0.3)';
        ctx.font = '9px JetBrains Mono, monospace';
        ctx.textAlign = 'center';
        const labelStep = Math.max(1, Math.floor(data.length / 6));
        data.forEach((d, i) => {
            if (i % labelStep === 0 || i === data.length - 1) {
                const x = padding.left + (chartW / Math.max(1, data.length - 1)) * i;
                const dateStr = new Date(d.date).toLocaleDateString('tr-TR', { day: '2-digit', month: 'short' });
                ctx.fillText(dateStr, x, h - 8);
            }
        });

        if (data.length < 2) return;

        const gradient = ctx.createLinearGradient(0, padding.top, 0, padding.top + chartH);
        gradient.addColorStop(0, 'rgba(139,92,246,0.3)');
        gradient.addColorStop(1, 'rgba(139,92,246,0)');

        ctx.beginPath();
        data.forEach((d, i) => {
            const x = padding.left + (chartW / (data.length - 1)) * i;
            const y = padding.top + chartH - ((values[i] - minVal) / range) * chartH;
            if (i === 0) ctx.moveTo(x, y);
            else ctx.lineTo(x, y);
        });
        ctx.lineTo(padding.left + chartW, padding.top + chartH);
        ctx.lineTo(padding.left, padding.top + chartH);
        ctx.closePath();
        ctx.fillStyle = gradient;
        ctx.fill();

        ctx.beginPath();
        data.forEach((d, i) => {
            const x = padding.left + (chartW / (data.length - 1)) * i;
            const y = padding.top + chartH - ((values[i] - minVal) / range) * chartH;
            if (i === 0) ctx.moveTo(x, y);
            else ctx.lineTo(x, y);
        });
        ctx.strokeStyle = '#8b5cf6';
        ctx.lineWidth = 2;
        ctx.stroke();

        data.forEach((d, i) => {
            const x = padding.left + (chartW / (data.length - 1)) * i;
            const y = padding.top + chartH - ((values[i] - minVal) / range) * chartH;
            ctx.beginPath();
            ctx.arc(x, y, 3, 0, Math.PI * 2);
            ctx.fillStyle = '#8b5cf6';
            ctx.fill();
            ctx.strokeStyle = '#1a1a2e';
            ctx.lineWidth = 1.5;
            ctx.stroke();
        });
    }

    getFilteredBadges(): BadgeShowcase[] {
        if (!this.badgeGallery) return [];
        switch (this.galleryFilter) {
            case 'earned': return this.badgeGallery.earnedBadges;
            case 'locked': return this.badgeGallery.lockedBadges;
            default: return [...this.badgeGallery.earnedBadges, ...this.badgeGallery.lockedBadges];
        }
    }

    getRarityColor(rarity: string): string {
        const colors: Record<string, string> = {
            'Legendary': '#fbbf24',
            'Epic': '#a855f7',
            'Rare': '#3b82f6',
            'Uncommon': '#10b981',
            'Common': '#6b7280'
        };
        return colors[rarity] || colors['Common'];
    }

    getRarityLabel(rarity: string): string {
        const labels: Record<string, string> = {
            'Legendary': 'Efsanevi',
            'Epic': 'Destansı',
            'Rare': 'Nadir',
            'Uncommon': 'Sıra Dışı',
            'Common': 'Yaygın'
        };
        return labels[rarity] || rarity;
    }

    getBadgeEmoji(name: string): string {
        const map: Record<string, string> = {
            'İlk Adım': '👣', 'Prompt Ustası': '🧙', 'Tabu Kaçkını': '🎯',
            'Hızlı Eller': '⚡', 'Seri Katil': '🔥', 'Mükemmeliyetçi': '💎'
        };
        return map[name] || '🏅';
    }
}
