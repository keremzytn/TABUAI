import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserProfile } from '../../models/user.models';
import { Word } from '../../models/game.models';

export interface DashboardStats {
    totalUsers: number;
    activeUsers: number;
    inactiveUsers: number;
    totalGames: number;
    todayGames: number;
    totalWords: number;
    activeWords: number;
    inactiveWords: number;
    totalBadges: number;
    totalWordPacks: number;
    last7DaysRegistrations: DailyRegistration[];
    topUsers: TopUser[];
}

export interface DailyRegistration {
    date: string;
    count: number;
}

export interface TopUser {
    id: string;
    username: string;
    displayName: string;
    totalScore: number;
    gamesPlayed: number;
    gamesWon: number;
}

export interface UserDetail {
    id: string;
    username: string;
    email: string;
    displayName?: string;
    level: string;
    role: string;
    totalScore: number;
    gamesPlayed: number;
    gamesWon: number;
    winRate: number;
    promptCoins: number;
    currentStreak: number;
    bestStreak: number;
    isActive: boolean;
    createdAt: string;
    selectedAvatar?: string;
    selectedCardDesign?: string;
    badges: UserDetailBadge[];
    recentGames: UserDetailGame[];
    coinTransactions: UserDetailCoinTransaction[];
}

export interface UserDetailBadge {
    badgeId: string;
    name: string;
    description: string;
    iconUrl: string;
    earnedAt: string;
}

export interface UserDetailGame {
    id: string;
    targetWord: string;
    mode: string;
    status: string;
    isCorrectGuess: boolean;
    score: number;
    attemptNumber: number;
    startedAt: string;
}

export interface UserDetailCoinTransaction {
    id: string;
    amount: number;
    type: string;
    description: string;
    createdAt: string;
}

export interface BadgeAdmin {
    id: string;
    name: string;
    description: string;
    iconUrl: string;
    type: string;
    requiredValue: number;
    isActive: boolean;
    createdAt: string;
    userCount: number;
}

export interface WordPackAdmin {
    id: string;
    name: string;
    description: string;
    language: string;
    createdByUsername: string;
    isPublic: boolean;
    isApproved: boolean;
    playCount: number;
    likeCount: number;
    wordCount: number;
    createdAt: string;
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    limit: number;
    totalPages: number;
}

export interface GameSessionAdmin {
    id: string;
    username: string;
    targetWord: string;
    mode: string;
    status: string;
    isCorrectGuess: boolean;
    score: number;
    attemptNumber: number;
    startedAt: string;
    completedAt?: string;
}

export interface ActivityLogAdmin {
    id: string;
    username: string;
    type: string;
    description: string;
    scoreEarned?: number;
    createdAt: string;
}

@Injectable({
    providedIn: 'root'
})
export class AdminService {
    private apiUrl = `${environment.apiUrl}/admin`;

    constructor(private http: HttpClient) { }

    // Dashboard
    getDashboardStats(): Observable<DashboardStats> {
        return this.http.get<DashboardStats>(`${this.apiUrl}/dashboard`);
    }

    // Users
    getAllUsers(): Observable<UserProfile[]> {
        return this.http.get<UserProfile[]>(`${this.apiUrl}/users`);
    }

    getUserDetail(userId: string): Observable<UserDetail> {
        return this.http.get<UserDetail>(`${this.apiUrl}/users/${userId}`);
    }

    toggleUserStatus(userId: string): Observable<boolean> {
        return this.http.post<boolean>(`${this.apiUrl}/users/${userId}/toggle-status`, {});
    }

    // Words
    getAllWords(): Observable<Word[]> {
        return this.http.get<Word[]>(`${this.apiUrl}/words`);
    }

    addWord(word: Word): Observable<string> {
        return this.http.post<string>(`${this.apiUrl}/words`, word);
    }

    updateWord(word: Word): Observable<boolean> {
        return this.http.put<boolean>(`${this.apiUrl}/words`, word);
    }

    deleteWord(id: string): Observable<boolean> {
        return this.http.delete<boolean>(`${this.apiUrl}/words/${id}`);
    }

    // Badges
    getAllBadges(): Observable<BadgeAdmin[]> {
        return this.http.get<BadgeAdmin[]>(`${this.apiUrl}/badges`);
    }

    createBadge(badge: { name: string; description: string; iconUrl: string; type: number; requiredValue: number }): Observable<string> {
        return this.http.post<string>(`${this.apiUrl}/badges`, badge);
    }

    updateBadge(badge: { id: string; name: string; description: string; iconUrl: string; type: number; requiredValue: number; isActive: boolean }): Observable<boolean> {
        return this.http.put<boolean>(`${this.apiUrl}/badges/${badge.id}`, badge);
    }

    deleteBadge(id: string): Observable<boolean> {
        return this.http.delete<boolean>(`${this.apiUrl}/badges/${id}`);
    }

    assignBadge(badgeId: string, userId: string): Observable<boolean> {
        return this.http.post<boolean>(`${this.apiUrl}/badges/${badgeId}/assign/${userId}`, {});
    }

    // Word Packs
    getAllWordPacks(): Observable<WordPackAdmin[]> {
        return this.http.get<WordPackAdmin[]>(`${this.apiUrl}/word-packs`);
    }

    createWordPack(pack: { name: string; description: string; language: string; isPublic: boolean }): Observable<string> {
        return this.http.post<string>(`${this.apiUrl}/word-packs`, pack);
    }

    updateWordPack(pack: { id: string; name: string; description: string; language: string; isPublic: boolean; isApproved: boolean }): Observable<boolean> {
        return this.http.put<boolean>(`${this.apiUrl}/word-packs/${pack.id}`, pack);
    }

    deleteWordPack(id: string): Observable<boolean> {
        return this.http.delete<boolean>(`${this.apiUrl}/word-packs/${id}`);
    }

    // Activity
    getGameSessions(page: number = 1, limit: number = 50): Observable<PagedResult<GameSessionAdmin>> {
        const params = new HttpParams().set('page', page).set('limit', limit);
        return this.http.get<PagedResult<GameSessionAdmin>>(`${this.apiUrl}/game-sessions`, { params });
    }

    getActivityLogs(page: number = 1, limit: number = 100): Observable<PagedResult<ActivityLogAdmin>> {
        const params = new HttpParams().set('page', page).set('limit', limit);
        return this.http.get<PagedResult<ActivityLogAdmin>>(`${this.apiUrl}/activity-logs`, { params });
    }
}
