import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ActivityService } from '../../services/activity.service';
import { AuthService } from '../../services/auth.service';
import { ActivityLog } from '../../models/versus.models';

@Component({
  selector: 'app-activity',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './activity.component.html',
  styleUrl: './activity.component.scss'
})
export class ActivityComponent implements OnInit {
  activities: ActivityLog[] = [];
  loading = false;
  page = 1;
  hasMore = true;

  constructor(
    private activityService: ActivityService,
    private authService: AuthService,
    private router: Router
  ) {}

  get userId(): string {
    return this.authService.currentUserValue?.id ?? '';
  }

  ngOnInit(): void {
    if (!this.userId) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadFeed();
  }

  loadFeed(): void {
    this.loading = true;
    this.activityService.getFeed(this.userId, this.page).subscribe({
      next: (data) => {
        this.activities = [...this.activities, ...data];
        this.hasMore = data.length >= 20;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  loadMore(): void {
    this.page++;
    this.loadFeed();
  }

  getActivityIcon(type: string): string {
    const icons: Record<string, string> = {
      'GameWon': '\u{1F3C6}',
      'GameLost': '\u{1F614}',
      'VersusWon': '\u{2694}\uFE0F',
      'VersusLost': '\u{1F625}',
      'VersusDraw': '\u{1F91D}',
      'ChallengeReceived': '\u{1F4E8}',
      'ChallengeSent': '\u{1F4E4}',
      'BadgeEarned': '\u{1F3C5}',
      'LevelUp': '\u{2B06}\uFE0F'
    };
    return icons[type] || '\u{1F3AE}';
  }

  getTimeAgo(dateStr: string): string {
    const date = new Date(dateStr);
    const now = new Date();
    const diff = Math.floor((now.getTime() - date.getTime()) / 1000);
    if (diff < 60) return 'Az \u00F6nce';
    if (diff < 3600) return `${Math.floor(diff / 60)} dk \u00F6nce`;
    if (diff < 86400) return `${Math.floor(diff / 3600)} saat \u00F6nce`;
    return `${Math.floor(diff / 86400)} g\u00FCn \u00F6nce`;
  }
}
