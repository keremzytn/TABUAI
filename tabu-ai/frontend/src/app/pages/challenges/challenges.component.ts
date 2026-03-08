import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ChallengeService } from '../../services/challenge.service';
import { FriendService } from '../../services/friend.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { Challenge } from '../../models/versus.models';
import { Friend } from '../../models/friend.models';

@Component({
  selector: 'app-challenges',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './challenges.component.html',
  styleUrl: './challenges.component.scss'
})
export class ChallengesComponent implements OnInit {
  activeTab: 'received' | 'sent' | 'new' = 'received';
  pendingChallenges: Challenge[] = [];
  sentChallenges: Challenge[] = [];
  friends: Friend[] = [];
  loading = false;

  // New challenge form
  selectedFriendId = '';
  challengeMessage = '';
  selectedCategory: string | null = null;
  categories = ['Ula\u015F\u0131m', 'Teknoloji', 'Bilim', 'Sanat', 'Yemek', 'Spor', 'Tarih', 'Do\u011Fa', 'M\u00FCzik'];

  constructor(
    private challengeService: ChallengeService,
    private friendService: FriendService,
    private authService: AuthService,
    private toastService: ToastService,
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
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.challengeService.getPendingChallenges(this.userId).subscribe({
      next: (data) => { this.pendingChallenges = data; this.loading = false; },
      error: () => { this.loading = false; }
    });

    this.challengeService.getSentChallenges(this.userId).subscribe({
      next: (data) => this.sentChallenges = data
    });

    this.friendService.getFriends(this.userId).subscribe({
      next: (data) => this.friends = data
    });
  }

  acceptChallenge(challenge: Challenge): void {
    this.challengeService.respondToChallenge(challenge.id, this.userId, true).subscribe({
      next: (response: any) => {
        this.toastService.success('Meydan okuma kabul edildi!');
        if (response.roomCode) {
          this.router.navigate(['/versus'], { queryParams: { room: response.roomCode } });
        }
        this.loadData();
      },
      error: () => this.toastService.error('Bir hata olu\u015Ftu.')
    });
  }

  rejectChallenge(challenge: Challenge): void {
    this.challengeService.respondToChallenge(challenge.id, this.userId, false).subscribe({
      next: () => {
        this.toastService.info('Meydan okuma reddedildi.');
        this.loadData();
      }
    });
  }

  sendChallenge(): void {
    if (!this.selectedFriendId) {
      this.toastService.warning('Bir arkada\u015F se\u00E7melisiniz.');
      return;
    }

    this.challengeService.sendChallenge({
      challengerId: this.userId,
      challengedId: this.selectedFriendId,
      category: this.selectedCategory ?? undefined,
      message: this.challengeMessage || undefined
    }).subscribe({
      next: () => {
        this.toastService.success('Meydan okuma g\u00F6nderildi!');
        this.selectedFriendId = '';
        this.challengeMessage = '';
        this.selectedCategory = null;
        this.activeTab = 'sent';
        this.loadData();
      },
      error: () => this.toastService.error('G\u00F6nderilemedi.')
    });
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

  getStatusText(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'Bekliyor',
      'Accepted': 'Kabul Edildi',
      'Rejected': 'Reddedildi',
      'Expired': 'S\u00FCresi Doldu'
    };
    return map[status] || status;
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'pending',
      'Accepted': 'accepted',
      'Rejected': 'rejected',
      'Expired': 'expired'
    };
    return map[status] || '';
  }
}
