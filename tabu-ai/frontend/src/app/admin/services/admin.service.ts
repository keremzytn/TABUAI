import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserProfile } from '../../models/user.models';
import { Word } from '../../models/game.models';

@Injectable({
    providedIn: 'root'
})
export class AdminService {
    private apiUrl = `${environment.apiUrl}/admin`;

    constructor(private http: HttpClient) { }

    getAllUsers(): Observable<UserProfile[]> {
        return this.http.get<UserProfile[]>(`${this.apiUrl}/users`);
    }

    toggleUserStatus(userId: string): Observable<boolean> {
        return this.http.post<boolean>(`${this.apiUrl}/users/${userId}/toggle-status`, {});
    }

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
}
