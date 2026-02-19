import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface LeaderboardEntry {
    rank: number;
    userId: string;
    username: string;
    displayName?: string;
    level: string;
    totalScore: number;
    gamesPlayed: number;
    gamesWon: number;
    winRate: number;
}

export interface LeaderboardResponse {
    entries: LeaderboardEntry[];
    currentUser?: LeaderboardEntry;
    period: string;
    totalPlayers: number;
}

@Injectable({
    providedIn: 'root'
})
export class LeaderboardService {
    private readonly baseUrl = `${environment.apiUrl}/leaderboard`;

    constructor(private http: HttpClient) { }

    getLeaderboard(period: string = 'AllTime', userId?: string, top: number = 20): Observable<LeaderboardResponse> {
        let params = new HttpParams()
            .set('period', period)
            .set('top', top.toString());

        if (userId) {
            params = params.set('userId', userId);
        }

        return this.http.get<LeaderboardResponse>(this.baseUrl, { params });
    }
}
