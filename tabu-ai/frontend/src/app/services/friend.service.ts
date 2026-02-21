import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Friend, FriendRequest, UserSearchResult } from '../models/friend.models';

@Injectable({
    providedIn: 'root'
})
export class FriendService {
    private apiUrl = `${environment.apiUrl}/friends`;

    constructor(private http: HttpClient) { }

    getFriends(userId: string): Observable<Friend[]> {
        return this.http.get<Friend[]>(`${this.apiUrl}/${userId}`);
    }

    getPendingRequests(userId: string): Observable<FriendRequest[]> {
        return this.http.get<FriendRequest[]>(`${this.apiUrl}/${userId}/pending`);
    }

    searchUsers(term: string, userId: string): Observable<UserSearchResult[]> {
        return this.http.get<UserSearchResult[]>(`${this.apiUrl}/search`, {
            params: { term, userId }
        });
    }

    sendFriendRequest(requesterId: string, addresseeId: string): Observable<string> {
        return this.http.post<string>(`${this.apiUrl}/request`, {
            requesterId,
            addresseeId
        });
    }

    acceptRequest(friendshipId: string, userId: string): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/${friendshipId}/accept`, null, {
            params: { userId }
        });
    }

    rejectRequest(friendshipId: string, userId: string): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/${friendshipId}/reject`, null, {
            params: { userId }
        });
    }

    removeFriend(friendshipId: string, userId: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${friendshipId}`, {
            params: { userId }
        });
    }
}
