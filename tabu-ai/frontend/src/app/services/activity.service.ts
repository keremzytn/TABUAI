import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ActivityLog } from '../models/versus.models';

@Injectable({
  providedIn: 'root'
})
export class ActivityService {
  private readonly apiUrl = `${environment.apiUrl}/activity`;

  constructor(private http: HttpClient) {}

  getFeed(userId: string, page = 1, pageSize = 20): Observable<ActivityLog[]> {
    return this.http.get<ActivityLog[]>(`${this.apiUrl}/feed/${userId}`, {
      params: { page: page.toString(), pageSize: pageSize.toString() }
    });
  }

  getUserActivity(userId: string, page = 1, pageSize = 20): Observable<ActivityLog[]> {
    return this.http.get<ActivityLog[]>(`${this.apiUrl}/user/${userId}`, {
      params: { page: page.toString(), pageSize: pageSize.toString() }
    });
  }
}
