import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { WordPack, CreateWordPackRequest } from '../models/game.models';

@Injectable({
  providedIn: 'root'
})
export class WordPackService {
  private readonly baseUrl = `${environment.apiUrl}/wordpacks`;

  constructor(private http: HttpClient) {}

  getPublicPacks(language: string = 'tr'): Observable<WordPack[]> {
    return this.http.get<WordPack[]>(`${this.baseUrl}?language=${language}`);
  }

  getPackDetail(id: string): Observable<WordPack> {
    return this.http.get<WordPack>(`${this.baseUrl}/${id}`);
  }

  getMyPacks(userId: string): Observable<WordPack[]> {
    return this.http.get<WordPack[]>(`${this.baseUrl}/my?userId=${userId}`);
  }

  createPack(request: CreateWordPackRequest, userId: string): Observable<WordPack> {
    return this.http.post<WordPack>(`${this.baseUrl}?userId=${userId}`, request);
  }

  likePack(id: string): Observable<{ likeCount: number }> {
    return this.http.post<{ likeCount: number }>(`${this.baseUrl}/${id}/like`, {});
  }

  deletePack(id: string, userId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}?userId=${userId}`);
  }
}
