import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Challenge, SendChallengeRequest } from '../models/versus.models';

@Injectable({
  providedIn: 'root'
})
export class ChallengeService {
  private readonly apiUrl = `${environment.apiUrl}/versus`;

  constructor(private http: HttpClient) {}

  sendChallenge(request: SendChallengeRequest): Observable<Challenge> {
    return this.http.post<Challenge>(`${this.apiUrl}/challenge`, request);
  }

  getPendingChallenges(userId: string): Observable<Challenge[]> {
    return this.http.get<Challenge[]>(`${this.apiUrl}/challenges/${userId}/pending`);
  }

  getSentChallenges(userId: string): Observable<Challenge[]> {
    return this.http.get<Challenge[]>(`${this.apiUrl}/challenges/${userId}/sent`);
  }

  respondToChallenge(challengeId: string, userId: string, accept: boolean): Observable<any> {
    return this.http.put(`${this.apiUrl}/challenges/${challengeId}/respond`, {
      userId,
      accept
    });
  }
}
