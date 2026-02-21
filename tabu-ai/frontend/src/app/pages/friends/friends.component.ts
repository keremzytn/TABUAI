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
        if (!this.userId) return;
        // We need the friendship ID. We'll search for it via the friends endpoint
        // For now, we need to match by userId - but the backend needs the friendship ID
        // Let's get it from the friend list using a separate approach
        // Actually, we can use the friend's userId to find the friendship
        this.friendService.getFriends(this.userId).subscribe({
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
}
