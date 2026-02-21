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
  templateUrl: './leaderboard.component.html',
  styleUrl: './leaderboard.component.scss'
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