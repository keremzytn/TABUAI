import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container">
      <div class="leaderboard-header">
        <h1>🏆 Liderlik Tablosu</h1>
        <p>En başarılı prompt yazarları</p>
      </div>

      <div class="leaderboard-content">
        <!-- Top 3 Podium -->
        <div class="podium">
          <div class="podium-place second">
            <div class="podium-medal">🥈</div>
            <h3>Alice</h3>
            <p class="score">1,850</p>
            <span class="level">Usta</span>
          </div>
          
          <div class="podium-place first">
            <div class="podium-medal">🥇</div>
            <h3>Bob</h3>
            <p class="score">2,340</p>
            <span class="level">Büyük Usta</span>
          </div>
          
          <div class="podium-place third">
            <div class="podium-medal">🥉</div>
            <h3>Charlie</h3>
            <p class="score">1,620</p>
            <span class="level">Uzman</span>
          </div>
        </div>

        <!-- Leaderboard Table -->
        <div class="leaderboard-table">
          <div class="table-header">
            <div class="col-rank">Sıra</div>
            <div class="col-player">Oyuncu</div>
            <div class="col-score">Puan</div>
            <div class="col-games">Oyun</div>
            <div class="col-rate">Başarı</div>
          </div>

          <div *ngFor="let player of leaderboardData; let i = index" 
               class="table-row" 
               [class.top-three]="i < 3">
            <div class="col-rank">
              <span class="rank-number">{{ i + 1 }}</span>
              <span class="rank-icon">{{ getRankIcon(i) }}</span>
            </div>
            <div class="col-player">
              <span class="player-name">{{ player.name }}</span>
              <span class="player-level">{{ player.level }}</span>
            </div>
            <div class="col-score">{{ player.score.toLocaleString() }}</div>
            <div class="col-games">{{ player.gamesPlayed }}</div>
            <div class="col-rate">{{ player.winRate }}%</div>
          </div>
        </div>

        <!-- Call to Action -->
        <div class="cta-section">
          <h2>Sen de Yarışmaya Katıl!</h2>
          <a routerLink="/game" class="btn btn-primary btn-large">
            🎯 Hemen Oyna
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .leaderboard-header {
      text-align: center;
      margin-bottom: 48px;
    }

    .leaderboard-header h1 {
      font-size: 2.5rem;
      font-weight: 700;
      margin-bottom: 8px;
      color: #1a202c;
    }

    .leaderboard-header p {
      font-size: 1rem;
      color: #718096;
    }

    /* Podium */
    .podium {
      display: flex;
      justify-content: center;
      align-items: flex-end;
      gap: 16px;
      margin-bottom: 48px;
    }

    .podium-place {
      background: white;
      border: 2px solid #e2e8f0;
      border-radius: 12px;
      padding: 24px 20px;
      text-align: center;
      transition: all 0.2s ease;
      min-width: 160px;
    }

    .podium-place:hover {
      transform: translateY(-4px);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }

    .podium-place.first {
      order: 2;
      border-color: #ffd700;
      transform: scale(1.05);
    }

    .podium-place.second {
      order: 1;
      border-color: #c0c0c0;
    }

    .podium-place.third {
      order: 3;
      border-color: #cd7f32;
    }

    .podium-medal {
      font-size: 3rem;
      margin-bottom: 12px;
    }

    .podium-place h3 {
      font-size: 1.25rem;
      font-weight: 600;
      margin-bottom: 8px;
      color: #2d3748;
    }

    .podium-place .score {
      font-size: 1.125rem;
      font-weight: 700;
      color: #667eea;
      margin-bottom: 8px;
    }

    .podium-place .level {
      background: #667eea;
      color: white;
      padding: 4px 12px;
      border-radius: 6px;
      font-size: 0.875rem;
      font-weight: 600;
      display: inline-block;
    }

    /* Table */
    .leaderboard-table {
      background: white;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      padding: 24px;
      margin-bottom: 48px;
    }

    .table-header {
      display: grid;
      grid-template-columns: 80px 1fr 100px 80px 80px;
      gap: 16px;
      padding: 12px 16px;
      background: #f7fafc;
      border-radius: 8px;
      font-weight: 600;
      font-size: 0.875rem;
      color: #4a5568;
      margin-bottom: 8px;
    }

    .table-row {
      display: grid;
      grid-template-columns: 80px 1fr 100px 80px 80px;
      gap: 16px;
      padding: 16px;
      align-items: center;
      border-bottom: 1px solid #f7fafc;
      transition: background-color 0.2s ease;
    }

    .table-row:hover {
      background-color: #f7fafc;
    }

    .table-row.top-three {
      background: #faf5ff;
    }

    .col-rank {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .rank-number {
      font-weight: 600;
      color: #2d3748;
    }

    .rank-icon {
      font-size: 1.125rem;
    }

    .col-player {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .player-name {
      font-weight: 500;
      color: #2d3748;
    }

    .player-level {
      font-size: 0.875rem;
      color: #718096;
    }

    .col-score {
      font-weight: 700;
      color: #48bb78;
    }

    .col-games {
      color: #4a5568;
    }

    .col-rate {
      font-weight: 600;
      color: #667eea;
    }

    /* CTA */
    .cta-section {
      text-align: center;
      padding: 48px 32px;
      background: #f7fafc;
      border-radius: 12px;
    }

    .cta-section h2 {
      font-size: 1.75rem;
      font-weight: 700;
      margin-bottom: 24px;
      color: #1a202c;
    }

    .btn-large {
      padding: 14px 32px;
      font-size: 1rem;
    }

    /* Responsive */
    @media (max-width: 768px) {
      .leaderboard-header h1 {
        font-size: 2rem;
      }

      .podium {
        flex-direction: column;
        align-items: center;
      }

      .podium-place.first {
        order: 1;
        transform: none;
      }

      .podium-place.second {
        order: 2;
      }

      .podium-place.third {
        order: 3;
      }

      .table-header,
      .table-row {
        grid-template-columns: 60px 1fr 80px;
        gap: 12px;
      }

      .col-games,
      .col-rate {
        display: none;
      }

      .cta-section {
        padding: 32px 20px;
      }

      .cta-section h2 {
        font-size: 1.5rem;
      }
    }
  `]
})
export class LeaderboardComponent {
  leaderboardData = [
    { name: 'Bob', score: 2340, gamesPlayed: 45, winRate: 87, level: 'Büyük Usta' },
    { name: 'Alice', score: 1850, gamesPlayed: 38, winRate: 82, level: 'Usta' },
    { name: 'Charlie', score: 1620, gamesPlayed: 42, winRate: 76, level: 'Uzman' },
    { name: 'Diana', score: 1480, gamesPlayed: 35, winRate: 79, level: 'Becerikli' },
    { name: 'Eve', score: 1320, gamesPlayed: 29, winRate: 83, level: 'Becerikli' },
    { name: 'Frank', score: 1180, gamesPlayed: 31, winRate: 71, level: 'Çırak' },
    { name: 'Grace', score: 1050, gamesPlayed: 27, winRate: 74, level: 'Çırak' },
    { name: 'Henry', score: 920, gamesPlayed: 25, winRate: 68, level: 'Çaylak' },
    { name: 'Ivy', score: 860, gamesPlayed: 23, winRate: 72, level: 'Çaylak' },
    { name: 'Jack', score: 750, gamesPlayed: 21, winRate: 65, level: 'Çaylak' }
  ];

  getRankIcon(index: number): string {
    if (index === 0) return '👑';
    if (index === 1) return '🥈';
    if (index === 2) return '🥉';
    if (index < 10) return '⭐';
    return '';
  }
}