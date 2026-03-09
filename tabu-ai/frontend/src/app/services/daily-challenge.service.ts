import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { DailyChallenge, DailyChallengeResult, DailyChallengeLeaderboard } from '../models/game.models';

@Injectable({
  providedIn: 'root'
})
export class DailyChallengeService {
  private readonly baseUrl = `${environment.apiUrl}/daily-challenge`;

  constructor(private http: HttpClient) {}

  getTodaysChallenge(language: string = 'tr', userId?: string): Observable<DailyChallenge> {
    let url = `${this.baseUrl}/today?language=${language}`;
    if (userId) url += `&userId=${userId}`;
    return this.http.get<DailyChallenge>(url);
  }

  completeChallenge(userId: string, dailyChallengeId: string, gameSessionId: string): Observable<DailyChallengeResult> {
    return this.http.post<DailyChallengeResult>(`${this.baseUrl}/complete`, {
      userId, dailyChallengeId, gameSessionId
    });
  }

  getLeaderboard(dailyChallengeId?: string, language: string = 'tr'): Observable<DailyChallengeLeaderboard[]> {
    let url = `${this.baseUrl}/leaderboard?language=${language}`;
    if (dailyChallengeId) url += `&dailyChallengeId=${dailyChallengeId}`;
    return this.http.get<DailyChallengeLeaderboard[]>(url);
  }
}
