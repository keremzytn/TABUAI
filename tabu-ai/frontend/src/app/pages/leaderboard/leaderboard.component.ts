import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LeaderboardService, LeaderboardEntry, LeaderboardResponse } from '../../services/leaderboard.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container leaderboard-container">
      <div class="header-section text-center fade-in">
        <h1 class="page-title">
          🏆 <span class="text-gradient">Liderlik Tablosu</span>
        </h1>
        <p class="text-muted">En başarılı prompt yazarları</p>
      </div>

      <!-- Period Tabs -->
      <div class="period-tabs fade-in">
        <button *ngFor="let p of periods" 
                class="tab-btn"
                [class.active]="selectedPeriod === p.key"
                (click)="changePeriod(p.key)">
          {{ p.label }}
        </button>
      </div>

      <!-- Loading -->
      <div *ngIf="loading" class="loading-section text-center">
        <div class="loading-spinner large"></div>
        <p class="text-muted mt-4">Yükleniyor...</p>
      </div>

      <ng-container *ngIf="!loading">
        <!-- Top 3 Podium -->
        <div class="podium-grid fade-in" *ngIf="topThree.length > 0">
          <!-- 2nd Place -->
          <div *ngIf="topThree.length >= 2" class="podium-card glass-card second pop-in" style="animation-delay: 0.1s">
            <div class="rank-badge">🥈</div>
            <div class="player-info">
              <h3>{{ topThree[1].displayName || topThree[1].username }}</h3>
              <span class="level-badge">{{ getLevelText(topThree[1].level) }}</span>
            </div>
            <div class="score">{{ topThree[1].totalScore | number }}</div>
            <div class="win-rate">{{ topThree[1].winRate | number:'1.0-0' }}% Başarı</div>
          </div>
          
          <!-- 1st Place -->
          <div *ngIf="topThree.length >= 1" class="podium-card glass-card first pop-in">
            <div class="crown">👑</div>
            <div class="rank-badge gold">🥇</div>
            <div class="player-info">
              <h3>{{ topThree[0].displayName || topThree[0].username }}</h3>
              <span class="level-badge highlight">{{ getLevelText(topThree[0].level) }}</span>
            </div>
            <div class="score">{{ topThree[0].totalScore | number }}</div>
            <div class="win-rate">{{ topThree[0].winRate | number:'1.0-0' }}% Başarı</div>
          </div>
          
          <!-- 3rd Place -->
          <div *ngIf="topThree.length >= 3" class="podium-card glass-card third pop-in" style="animation-delay: 0.2s">
            <div class="rank-badge">🥉</div>
            <div class="player-info">
              <h3>{{ topThree[2].displayName || topThree[2].username }}</h3>
              <span class="level-badge">{{ getLevelText(topThree[2].level) }}</span>
            </div>
            <div class="score">{{ topThree[2].totalScore | number }}</div>
            <div class="win-rate">{{ topThree[2].winRate | number:'1.0-0' }}% Başarı</div>
          </div>
        </div>

        <!-- Current User Position -->
        <div *ngIf="currentUserEntry" class="current-user-card glass-card fade-in">
          <div class="your-rank">
            <span class="rank-label">Senin Sıran</span>
            <span class="rank-number">#{{ currentUserEntry.rank }}</span>
          </div>
          <div class="your-score">
            <span class="score-value">{{ currentUserEntry.totalScore | number }}</span>
            <span class="score-label">puan</span>
          </div>
        </div>

        <!-- Leaderboard List -->
        <div class="ranking-list glass-card fade-in" *ngIf="remainingPlayers.length > 0">
          <div class="list-header">
            <div class="col-rank">#</div>
            <div class="col-player">Oyuncu</div>
            <div class="col-score">Puan</div>
            <div class="col-stats mobile-hide">Oyun / Başarı</div>
          </div>

          <div *ngFor="let player of remainingPlayers" 
               class="list-row"
               [class.is-you]="currentUserEntry && player.userId === currentUserEntry.userId">
            <div class="col-rank">
              <span class="rank-num">{{ player.rank }}</span>
            </div>
            <div class="col-player">
              <span class="name">{{ player.displayName || player.username }}</span>
              <span class="level mobile-show">{{ getLevelText(player.level) }}</span>
            </div>
            <div class="col-score">{{ player.totalScore | number }}</div>
            <div class="col-stats mobile-hide">
              {{ player.gamesPlayed }} / {{ player.winRate | number:'1.0-0' }}%
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div *ngIf="allEntries.length === 0" class="empty-state glass-card fade-in text-center">
          <div class="empty-icon">🏆</div>
          <h3>Henüz oyuncu yok!</h3>
          <p class="text-muted">İlk sırayı almak için hemen oyna.</p>
          <a routerLink="/game" class="btn btn-primary mt-4">Oyna</a>
        </div>

        <!-- Stats Footer -->
        <div *ngIf="totalPlayers > 0" class="stats-footer text-center fade-in">
          <span class="text-muted">Toplam {{ totalPlayers }} oyuncu</span>
        </div>
      </ng-container>

      <div class="action-area text-center mt-4 fade-in">
        <a routerLink="/game" class="btn btn-primary pulse">
          🎮 Sıralamaya Gir
        </a>
      </div>
    </div>
  `,
  styles: [`
    .leaderboard-container {
      padding-top: 20px;
      padding-bottom: 100px;
    }

    .header-section {
      margin-bottom: 30px;
    }

    .page-title {
      font-size: 2rem;
      font-weight: 800;
      margin-bottom: 8px;
    }

    /* Period Tabs */
    .period-tabs {
      display: flex;
      justify-content: center;
      gap: 8px;
      margin-bottom: 36px;
    }
    .tab-btn {
      background: rgba(255, 255, 255, 0.05);
      border: 1px solid rgba(255, 255, 255, 0.1);
      color: var(--text-muted);
      padding: 8px 20px;
      border-radius: 20px;
      font-size: 0.85rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
    }
    .tab-btn:hover { background: rgba(255, 255, 255, 0.08); color: white; }
    .tab-btn.active { 
      background: rgba(139, 92, 246, 0.2); 
      border-color: var(--primary); 
      color: #c4b5fd; 
    }

    /* Loading */
    .loading-section {
      padding: 60px 0;
    }
    .large { width: 50px; height: 50px; border-width: 4px; }

    /* Podium */
    .podium-grid {
      display: flex;
      justify-content: center;
      align-items: flex-end;
      gap: 16px;
      margin-bottom: 30px;
      padding: 0 10px;
    }

    .podium-card {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 24px 16px;
      width: 100%;
      max-width: 160px;
      text-align: center;
      border: 1px solid rgba(255, 255, 255, 0.1);
      position: relative;
      transition: transform 0.3s;
    }
    .podium-card:hover { transform: translateY(-4px); }

    .crown {
      position: absolute;
      top: -20px;
      font-size: 2rem;
      animation: float 3s ease-in-out infinite;
    }
    @keyframes float {
      0%, 100% { transform: translateY(0); }
      50% { transform: translateY(-6px); }
    }

    .rank-badge {
      font-size: 2rem;
      margin-bottom: 12px;
    }

    /* 1st Place */
    .podium-card.first {
      height: 240px;
      background: linear-gradient(to bottom, rgba(234, 179, 8, 0.08), rgba(30, 41, 59, 0.6));
      border-color: rgba(234, 179, 8, 0.4);
      z-index: 2;
      transform: scale(1.08);
    }
    .first .score { color: #eab308; font-size: 1.4rem; }

    /* 2nd Place */
    .podium-card.second {
      height: 210px;
      background: linear-gradient(to bottom, rgba(148, 163, 184, 0.08), rgba(30, 41, 59, 0.6));
      border-color: rgba(148, 163, 184, 0.4);
    }
    .second .score { color: #94a3b8; }

    /* 3rd Place */
    .podium-card.third {
      height: 190px;
      background: linear-gradient(to bottom, rgba(180, 83, 9, 0.08), rgba(30, 41, 59, 0.6));
      border-color: rgba(180, 83, 9, 0.4);
    }
    .third .score { color: #d97706; }

    .player-info h3 {
      font-size: 0.95rem;
      font-weight: 700;
      margin-bottom: 4px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      max-width: 100%;
    }

    .level-badge {
      font-size: 0.7rem;
      padding: 3px 10px;
      background: rgba(255, 255, 255, 0.08);
      border-radius: 12px;
      display: inline-block;
      margin-bottom: 10px;
      font-weight: 600;
    }
    .level-badge.highlight { background: rgba(234, 179, 8, 0.15); color: #fde047; }

    .score {
      font-weight: 800;
      font-family: 'JetBrains Mono', monospace;
      font-size: 1.1rem;
      margin-top: auto;
    }

    .win-rate {
      font-size: 0.7rem;
      color: var(--text-muted);
      margin-top: 4px;
    }

    /* Current User Card */
    .current-user-card {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 18px 24px;
      margin-bottom: 20px;
      border-color: rgba(139, 92, 246, 0.3);
      background: rgba(139, 92, 246, 0.05);
    }
    .your-rank {
      display: flex;
      flex-direction: column;
    }
    .rank-label { font-size: 0.75rem; color: var(--text-muted); }
    .rank-number { font-size: 1.5rem; font-weight: 800; color: var(--primary); }
    .your-score {
      display: flex;
      align-items: baseline;
      gap: 4px;
    }
    .score-value { font-size: 1.5rem; font-weight: 800; color: var(--accent); }
    .score-label-small { font-size: 0.8rem; color: var(--text-muted); }

    /* List */
    .ranking-list {
      padding: 0;
      overflow: hidden;
    }

    .list-header {
      display: grid;
      grid-template-columns: 50px 2fr 1fr 1fr;
      padding: 14px 20px;
      background: rgba(0, 0, 0, 0.2);
      font-size: 0.8rem;
      color: var(--text-muted);
      text-transform: uppercase;
      letter-spacing: 1px;
      font-weight: 600;
    }

    .list-row {
      display: grid;
      grid-template-columns: 50px 2fr 1fr 1fr;
      padding: 14px 20px;
      align-items: center;
      border-bottom: 1px solid rgba(255, 255, 255, 0.04);
      transition: background 0.2s;
    }

    .list-row:last-child { border-bottom: none; }
    .list-row:hover { background: rgba(255, 255, 255, 0.03); }
    .list-row.is-you { 
      background: rgba(139, 92, 246, 0.08);
      border-left: 3px solid var(--primary);
    }

    .rank-num {
      font-weight: 700;
      color: var(--text-muted);
      font-size: 0.9rem;
    }

    .col-player .name {
      display: block;
      font-weight: 600;
      font-size: 0.95rem;
    }
    .col-player .level {
      font-size: 0.7rem;
      color: var(--text-muted);
    }

    .col-score {
      font-weight: 700;
      color: var(--primary);
      font-family: 'JetBrains Mono', monospace;
    }

    .col-stats {
      font-size: 0.85rem;
      color: var(--text-muted);
    }

    /* Empty State */
    .empty-state {
      padding: 60px 30px;
    }
    .empty-icon {
      font-size: 4rem;
      margin-bottom: 16px;
    }

    /* Stats Footer */
    .stats-footer {
      margin-top: 16px;
      font-size: 0.85rem;
    }

    /* Responsive */
    @media (max-width: 768px) {
      .mobile-hide { display: none; }
      .mobile-show { display: block; }
      
      .list-header, .list-row {
        grid-template-columns: 40px 1fr 80px;
      }
      
      .podium-grid {
        gap: 8px;
      }
      
      .podium-card {
        padding: 16px 8px;
        max-width: 120px;
      }

      .period-tabs {
        gap: 4px;
      }
      .tab-btn {
        padding: 6px 14px;
        font-size: 0.8rem;
      }
    }
  `]
})
export class LeaderboardComponent implements OnInit {
  allEntries: LeaderboardEntry[] = [];
  topThree: LeaderboardEntry[] = [];
  remainingPlayers: LeaderboardEntry[] = [];
  currentUserEntry: LeaderboardEntry | null = null;
  totalPlayers = 0;
  loading = false;
  selectedPeriod = 'AllTime';

  periods = [
    { key: 'Weekly', label: 'Haftalık' },
    { key: 'Monthly', label: 'Aylık' },
    { key: 'AllTime', label: 'Tüm Zamanlar' }
  ];

  // Fallback mock data
  private mockData: LeaderboardEntry[] = [
    { rank: 1, userId: '1', username: 'PromptMaster', level: 'GrandMaster', totalScore: 2340, gamesPlayed: 45, gamesWon: 39, winRate: 87 },
    { rank: 2, userId: '2', username: 'AIWhisperer', level: 'Master', totalScore: 1850, gamesPlayed: 38, gamesWon: 31, winRate: 82 },
    { rank: 3, userId: '3', username: 'WordSmith', level: 'Expert', totalScore: 1620, gamesPlayed: 42, gamesWon: 32, winRate: 76 },
    { rank: 4, userId: '4', username: 'TabuHunter', level: 'Skilled', totalScore: 1480, gamesPlayed: 35, gamesWon: 28, winRate: 79 },
    { rank: 5, userId: '5', username: 'CleverClue', level: 'Skilled', totalScore: 1320, gamesPlayed: 29, gamesWon: 24, winRate: 83 },
    { rank: 6, userId: '6', username: 'BrainStormer', level: 'Apprentice', totalScore: 1180, gamesPlayed: 31, gamesWon: 22, winRate: 71 },
    { rank: 7, userId: '7', username: 'Linguist42', level: 'Apprentice', totalScore: 1050, gamesPlayed: 27, gamesWon: 20, winRate: 74 },
    { rank: 8, userId: '8', username: 'QuickThinker', level: 'Rookie', totalScore: 920, gamesPlayed: 25, gamesWon: 17, winRate: 68 },
    { rank: 9, userId: '9', username: 'NoviceNinja', level: 'Rookie', totalScore: 860, gamesPlayed: 23, gamesWon: 17, winRate: 72 },
    { rank: 10, userId: '10', username: 'FirstTimer', level: 'Rookie', totalScore: 750, gamesPlayed: 21, gamesWon: 14, winRate: 65 }
  ];

  constructor(
    private leaderboardService: LeaderboardService,
    private authService: AuthService,
    private toastService: ToastService
  ) { }

  ngOnInit() {
    this.loadLeaderboard();
  }

  changePeriod(period: string) {
    this.selectedPeriod = period;
    this.loadLeaderboard();
  }

  loadLeaderboard() {
    this.loading = true;
    const userId = this.authService.currentUserValue?.id;

    this.leaderboardService.getLeaderboard(this.selectedPeriod, userId).subscribe({
      next: (response) => {
        this.processResponse(response);
        this.loading = false;
      },
      error: () => {
        // Fallback to mock data
        this.allEntries = this.mockData;
        this.topThree = this.allEntries.slice(0, 3);
        this.remainingPlayers = this.allEntries.slice(3);
        this.totalPlayers = this.allEntries.length;
        this.loading = false;
      }
    });
  }

  private processResponse(response: LeaderboardResponse) {
    this.allEntries = response.entries;
    this.topThree = this.allEntries.slice(0, 3);
    this.remainingPlayers = this.allEntries.slice(3);
    this.currentUserEntry = response.currentUser || null;
    this.totalPlayers = response.totalPlayers;
  }

  getLevelText(level: string): string {
    const map: Record<string, string> = {
      'Rookie': 'Çaylak',
      'Apprentice': 'Çırak',
      'Skilled': 'Becerikli',
      'Expert': 'Uzman',
      'Master': 'Usta',
      'GrandMaster': 'Büyük Usta'
    };
    return map[level] || level;
  }
}