import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { UserProfile, UserStatistic, GameHistory, PromptAnalysisChart, StyleAnalysis, BadgeGallery } from '../models/user.models';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private apiUrl = `${environment.apiUrl}/users`;

    constructor(private http: HttpClient) { }

    getUserProfile(userId: string): Observable<UserProfile> {
        return this.http.get<UserProfile>(`${this.apiUrl}/${userId}/profile`);
    }

    getUserStatistics(userId: string): Observable<UserStatistic[]> {
        return this.http.get<UserStatistic[]>(`${this.apiUrl}/${userId}/statistics`);
    }

    getUserGameHistory(userId: string, page: number = 1, pageSize: number = 10): Observable<GameHistory[]> {
        return this.http.get<GameHistory[]>(`${this.apiUrl}/${userId}/history`, {
            params: {
                page: page.toString(),
                pageSize: pageSize.toString()
            }
        });
    }

    getPromptAnalysis(userId: string, days: number = 30): Observable<PromptAnalysisChart> {
        return this.http.get<PromptAnalysisChart>(`${this.apiUrl}/${userId}/prompt-analysis`, {
            params: { days: days.toString() }
        });
    }

    getStyleAnalysis(userId: string): Observable<StyleAnalysis> {
        return this.http.get<StyleAnalysis>(`${this.apiUrl}/${userId}/style-analysis`);
    }

    getBadgeGallery(userId: string): Observable<BadgeGallery> {
        return this.http.get<BadgeGallery>(`${this.apiUrl}/${userId}/badge-gallery`);
    }
}
