import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { FriendService } from '../../services/friend.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { Friend, FriendRequest, UserSearchResult } from '../../models/friend.models';

@Component({
    selector: 'app-friends',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './friends.component.html',
    styleUrls: ['./friends.component.scss']
})
export class FriendsComponent implements OnInit {
    activeTab: 'friends' | 'pending' | 'search' = 'friends';
    userId: string | null = null;

    friends: Friend[] = [];
    pendingRequests: FriendRequest[] = [];
    searchResults: UserSearchResult[] = [];

    searchTerm = '';
    isLoading = true;
    isSearching = false;
    selectedFriend: Friend | null = null;

    private searchTimeout: any;

    constructor(
        private friendService: FriendService,
        private authService: AuthService,
        private toastService: ToastService
    ) { }

    ngOnInit(): void {
        const user = this.authService.currentUserValue;
        if (user) {
            this.userId = user.id;
            this.loadFriends();
            this.loadPendingRequests();
        } else {
            this.isLoading = false;
        }
    }

    switchTab(tab: 'friends' | 'pending' | 'search') {
        this.activeTab = tab;
    }

    loadFriends() {
        if (!this.userId) return;
        this.isLoading = true;
        this.friendService.getFriends(this.userId).subscribe({
            next: (data) => {
                this.friends = data;
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
            }
        });
    }

    loadPendingRequests() {
        if (!this.userId) return;
        this.friendService.getPendingRequests(this.userId).subscribe({
            next: (data) => this.pendingRequests = data,
            error: () => { }
        });
    }

    onSearchInput() {
        clearTimeout(this.searchTimeout);
        if (this.searchTerm.length < 2) {
            this.searchResults = [];
            return;
        }
        this.searchTimeout = setTimeout(() => this.searchUsers(), 400);
    }

    searchUsers() {
        if (!this.userId || this.searchTerm.length < 2) return;
        this.isSearching = true;
        this.friendService.searchUsers(this.searchTerm, this.userId).subscribe({
            next: (data) => {
                this.searchResults = data;
                this.isSearching = false;
            },
            error: () => {
                this.isSearching = false;
            }
        });
    }

    sendRequest(addresseeId: string) {
        if (!this.userId) return;
        this.friendService.sendFriendRequest(this.userId, addresseeId).subscribe({
            next: () => {
                this.toastService.success('Arkadaşlık isteği gönderildi! 🎉');
                // Update search result status
                const user = this.searchResults.find(u => u.userId === addresseeId);
                if (user) user.friendshipStatus = 'Pending';
            },
            error: (err) => {
                const msg = err.error?.message || err.error || 'İstek gönderilemedi.';
                this.toastService.error(typeof msg === 'string' ? msg : 'İstek gönderilemedi.');
            }
        });
    }

    acceptRequest(requestId: string) {
        if (!this.userId) return;
        this.friendService.acceptRequest(requestId, this.userId).subscribe({
            next: () => {
                this.toastService.success('Arkadaşlık isteği kabul edildi! 🤝');
                this.pendingRequests = this.pendingRequests.filter(r => r.requestId !== requestId);
                this.loadFriends();
            },
            error: () => this.toastService.error('İstek kabul edilemedi.')
        });
    }

    rejectRequest(requestId: string) {
        if (!this.userId) return;
        this.friendService.rejectRequest(requestId, this.userId).subscribe({
            next: () => {
                this.toastService.info('İstek reddedildi.');
                this.pendingRequests = this.pendingRequests.filter(r => r.requestId !== requestId);
            },
            error: () => this.toastService.error('İstek reddedilemedi.')
        });
    }

    removeFriend(friend: Friend) {
        if (!this.userId || !friend.friendshipId) return;
        this.friendService.removeFriend(friend.friendshipId, this.userId).subscribe({
            next: () => {
                this.toastService.info('Arkadaş kaldırıldı.');
                this.friends = this.friends.filter(f => f.userId !== friend.userId);
            },
            error: () => this.toastService.error('Arkadaş kaldırılamadı.')
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

    getInitial(name: string): string {
        return name ? name.charAt(0).toUpperCase() : '?';
    }

    openFriendPopup(friend: Friend) {
        this.selectedFriend = friend;
    }

    closeFriendPopup() {
        this.selectedFriend = null;
    }

    getFriendshipDuration(date: Date): string {
        const now = new Date();
        const then = new Date(date);
        const diffMs = now.getTime() - then.getTime();
        const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
        if (diffDays < 1) return 'Bugün';
        if (diffDays === 1) return '1 gün';
        if (diffDays < 30) return `${diffDays} gün`;
        const months = Math.floor(diffDays / 30);
        if (months < 12) return `${months} ay`;
        const years = Math.floor(months / 12);
        return `${years} yıl`;
    }
}
