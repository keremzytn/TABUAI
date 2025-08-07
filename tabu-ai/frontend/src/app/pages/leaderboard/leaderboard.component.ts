import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container">
      <div class="leaderboard-header fade-in">
        <h1>🏆 Liderlik Tablosu</h1>
        <p>En başarılı prompt yazarlarını keşfedin</p>
      </div>

      <div class="leaderboard-content">
        <!-- Top 3 -->
        <div class="podium fade-in">
          <div class="podium-place second">
            <div class="podium-medal">🥈</div>
            <div class="podium-info">
              <h3>Alice</h3>
              <p>1,850 Puan</p>
              <span class="level">Usta Promptçı</span>
            </div>
          </div>
          
          <div class="podium-place first">
            <div class="podium-medal">🥇</div>
            <div class="podium-info">
              <h3>Bob</h3>
              <p>2,340 Puan</p>
              <span class="level">Büyük Usta</span>
            </div>
          </div>
          
          <div class="podium-place third">
            <div class="podium-medal">🥉</div>
            <div class="podium-info">
              <h3>Charlie</h3>
              <p>1,620 Puan</p>
              <span class="level">Uzman</span>
            </div>
          </div>
        </div>

        <!-- Leaderboard Table -->
        <div class="leaderboard-table card fade-in">
          <h2>📊 Tüm Oyuncular</h2>
          
          <div class="table-container">
            <div class="table-row header">
              <div class="rank">Sıra</div>
              <div class="player">Oyuncu</div>
              <div class="score">Puan</div>
              <div class="games">Oyun</div>
              <div class="winrate">Başarı %</div>
              <div class="level">Seviye</div>
            </div>

            <div *ngFor="let player of leaderboardData; let i = index" 
                 class="table-row" 
                 [class.highlight]="i < 3">
              <div class="rank">
                <span class="rank-number">{{ i + 1 }}</span>
                <span class="rank-icon">{{ getRankIcon(i) }}</span>
              </div>
              <div class="player">
                <div class="player-info">
                  <span class="player-name">{{ player.name }}</span>
                  <span class="player-badges">{{ player.badges }}</span>
                </div>
              </div>
              <div class="score">{{ player.score.toLocaleString() }}</div>
              <div class="games">{{ player.gamesPlayed }}</div>
              <div class="winrate">{{ player.winRate }}%</div>
              <div class="level">{{ player.level }}</div>
            </div>
          </div>
        </div>

        <!-- Statistics -->
        <div class="stats-section">
          <div class="stat-card fade-in">
            <div class="stat-icon">👥</div>
            <div class="stat-info">
              <h3>2,547</h3>
              <p>Toplam Oyuncu</p>
            </div>
          </div>

          <div class="stat-card fade-in">
            <div class="stat-icon">🎮</div>
            <div class="stat-info">
              <h3>15,892</h3>
              <p>Oynanan Oyun</p>
            </div>
          </div>

          <div class="stat-card fade-in">
            <div class="stat-icon">📝</div>
            <div class="stat-info">
              <h3>42,156</h3>
              <p>Yazılan Prompt</p>
            </div>
          </div>

          <div class="stat-card fade-in">
            <div class="stat-icon">🤖</div>
            <div class="stat-info">
              <h3>67%</h3>
              <p>Ortalama Başarı</p>
            </div>
          </div>
        </div>

        <!-- Call to Action -->
        <div class="cta-section fade-in">
          <h2>🚀 Sen de Yarışmaya Katıl!</h2>
          <p>Prompt engineering becerilerini geliştir ve liderlik tablosunda yerini al</p>
          <a routerLink="/game" class="btn btn-primary btn-large pulse">
            🎯 Hemen Oyna
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .leaderboard-header {
      text-align: center;
      margin-bottom: 60px;
    }

    .leaderboard-header h1 {
      font-size: 3rem;
      margin-bottom: 16px;
      background: linear-gradient(45deg, #667eea, #764ba2);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .leaderboard-header p {
      font-size: 1.2rem;
      color: #6c757d;
    }

    .podium {
      display: flex;
      justify-content: center;
      align-items: end;
      gap: 16px;
      margin-bottom: 60px;
      flex-wrap: wrap;
    }

    .podium-place {
      background: white;
      border-radius: 16px;
      padding: 24px;
      text-align: center;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
      transition: transform 0.3s ease;
      min-width: 180px;
    }

    .podium-place:hover {
      transform: translateY(-5px);
    }

    .podium-place.first {
      order: 2;
      border: 3px solid #FFD700;
      transform: scale(1.1);
    }

    .podium-place.second {
      order: 1;
      border: 3px solid #C0C0C0;
    }

    .podium-place.third {
      order: 3;
      border: 3px solid #CD7F32;
    }

    .podium-medal {
      font-size: 3rem;
      margin-bottom: 16px;
    }

    .podium-info h3 {
      font-size: 1.5rem;
      margin-bottom: 8px;
      color: #333;
    }

    .podium-info p {
      font-size: 1.2rem;
      font-weight: bold;
      color: #667eea;
      margin-bottom: 8px;
    }

    .level {
      background: linear-gradient(45deg, #667eea, #764ba2);
      color: white;
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 0.9rem;
      font-weight: 500;
    }

    .leaderboard-table {
      margin-bottom: 60px;
    }

    .leaderboard-table h2 {
      margin-bottom: 24px;
      color: #333;
      text-align: center;
    }

    .table-container {
      overflow-x: auto;
    }

    .table-row {
      display: grid;
      grid-template-columns: 80px 1fr 120px 80px 100px 140px;
      gap: 16px;
      padding: 16px;
      align-items: center;
      border-bottom: 1px solid #e9ecef;
      transition: background-color 0.3s ease;
    }

    .table-row:hover {
      background-color: rgba(102, 126, 234, 0.05);
    }

    .table-row.header {
      background: #f8f9fa;
      font-weight: bold;
      color: #333;
      border-radius: 8px;
      margin-bottom: 8px;
    }

    .table-row.highlight {
      background: linear-gradient(45deg, rgba(102, 126, 234, 0.1), rgba(118, 75, 162, 0.1));
    }

    .rank {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .rank-number {
      font-weight: bold;
      color: #333;
    }

    .rank-icon {
      font-size: 1.2rem;
    }

    .player-info {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .player-name {
      font-weight: 500;
      color: #333;
    }

    .player-badges {
      font-size: 0.9rem;
    }

    .score {
      font-weight: bold;
      color: #51cf66;
    }

    .winrate {
      font-weight: 500;
      color: #667eea;
    }

    .stats-section {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 24px;
      margin-bottom: 60px;
    }

    .stat-card {
      background: white;
      border-radius: 16px;
      padding: 24px;
      text-align: center;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
      transition: transform 0.3s ease;
    }

    .stat-card:hover {
      transform: translateY(-5px);
    }

    .stat-icon {
      font-size: 2.5rem;
      margin-bottom: 16px;
    }

    .stat-info h3 {
      font-size: 2rem;
      font-weight: bold;
      color: #333;
      margin-bottom: 8px;
    }

    .stat-info p {
      color: #6c757d;
      font-weight: 500;
    }

    .cta-section {
      text-align: center;
      padding: 60px 40px;
      background: linear-gradient(135deg, rgba(102, 126, 234, 0.1), rgba(118, 75, 162, 0.1));
      border-radius: 30px;
    }

    .cta-section h2 {
      font-size: 2.5rem;
      margin-bottom: 16px;
      color: #333;
    }

    .cta-section p {
      font-size: 1.2rem;
      color: #6c757d;
      margin-bottom: 32px;
    }

    @media (max-width: 768px) {
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

      .table-row {
        grid-template-columns: 60px 1fr 80px;
        gap: 12px;
      }

      .games, .winrate, .level {
        display: none;
      }

      .stats-section {
        grid-template-columns: repeat(2, 1fr);
      }

      .cta-section {
        padding: 40px 20px;
      }

      .cta-section h2 {
        font-size: 2rem;
      }
    }
  `]
})
export class LeaderboardComponent {
  leaderboardData = [
    { name: 'Bob', score: 2340, gamesPlayed: 45, winRate: 87, level: 'Büyük Usta', badges: '🏆🎯🤖' },
    { name: 'Alice', score: 1850, gamesPlayed: 38, winRate: 82, level: 'Usta Promptçı', badges: '🎯🤖' },
    { name: 'Charlie', score: 1620, gamesPlayed: 42, winRate: 76, level: 'Uzman', badges: '🎯' },
    { name: 'Diana', score: 1480, gamesPlayed: 35, winRate: 79, level: 'Becerikli', badges: '🎮' },
    { name: 'Eve', score: 1320, gamesPlayed: 29, winRate: 83, level: 'Becerikli', badges: '🎮' },
    { name: 'Frank', score: 1180, gamesPlayed: 31, winRate: 71, level: 'Çırak', badges: '📝' },
    { name: 'Grace', score: 1050, gamesPlayed: 27, winRate: 74, level: 'Çırak', badges: '📝' },
    { name: 'Henry', score: 920, gamesPlayed: 25, winRate: 68, level: 'Çaylak', badges: '⭐' },
    { name: 'Ivy', score: 860, gamesPlayed: 23, winRate: 72, level: 'Çaylak', badges: '⭐' },
    { name: 'Jack', score: 750, gamesPlayed: 21, winRate: 65, level: 'Çaylak', badges: '⭐' }
  ];

  getRankIcon(index: number): string {
    if (index === 0) return '👑';
    if (index === 1) return '🥈';
    if (index === 2) return '🥉';
    if (index < 10) return '⭐';
    return '';
  }
}