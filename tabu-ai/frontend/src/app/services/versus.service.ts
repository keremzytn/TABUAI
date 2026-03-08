import { Injectable, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import {
  VersusGame,
  CreateRoomRequest,
  CreateRoomResponse,
  VersusGameStartedEvent,
  PlayerAttemptResultEvent,
  GameCompletedEvent
} from '../models/versus.models';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class VersusService implements OnDestroy {
  private readonly apiUrl = `${environment.apiUrl}/versus`;
  private hubConnection: signalR.HubConnection | null = null;

  // SignalR event streams
  private waitingForOpponent$ = new Subject<void>();
  private gameStarted$ = new Subject<VersusGameStartedEvent>();
  private playerAttemptResult$ = new Subject<PlayerAttemptResultEvent>();
  private gameCompleted$ = new Subject<GameCompletedEvent>();
  private tabuWordDetected$ = new Subject<{ detectedWords: string[] }>();
  private matchmakingError$ = new Subject<string>();
  private roomNotFound$ = new Subject<string>();
  private opponentDisconnected$ = new Subject<{ disconnectedPlayerId: string; winnerId: string | null }>();

  private connectionState$ = new BehaviorSubject<string>('disconnected');

  // Public observables
  onWaitingForOpponent = this.waitingForOpponent$.asObservable();
  onGameStarted = this.gameStarted$.asObservable();
  onPlayerAttemptResult = this.playerAttemptResult$.asObservable();
  onGameCompleted = this.gameCompleted$.asObservable();
  onTabuWordDetected = this.tabuWordDetected$.asObservable();
  onMatchmakingError = this.matchmakingError$.asObservable();
  onRoomNotFound = this.roomNotFound$.asObservable();
  onOpponentDisconnected = this.opponentDisconnected$.asObservable();
  onConnectionStateChange = this.connectionState$.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  // ---- SignalR Connection ----

  async startConnection(): Promise<void> {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) return;

    const baseUrl = environment.production ? '' : 'http://localhost:5000';

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${baseUrl}/hubs/game`, {
        accessTokenFactory: () => this.authService.token || ''
      })
      .withAutomaticReconnect()
      .build();

    this.registerHubEvents();

    try {
      await this.hubConnection.start();
      this.connectionState$.next('connected');
    } catch (err) {
      console.error('SignalR connection error:', err);
      this.connectionState$.next('error');
    }
  }

  async stopConnection(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.connectionState$.next('disconnected');
    }
  }

  private registerHubEvents(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('WaitingForOpponent', () => {
      this.waitingForOpponent$.next();
    });

    this.hubConnection.on('GameStarted', (data: VersusGameStartedEvent) => {
      this.gameStarted$.next(data);
    });

    this.hubConnection.on('PlayerAttemptResult', (data: PlayerAttemptResultEvent) => {
      this.playerAttemptResult$.next(data);
    });

    this.hubConnection.on('GameCompleted', (data: GameCompletedEvent) => {
      this.gameCompleted$.next(data);
    });

    this.hubConnection.on('TabuWordDetected', (data: { detectedWords: string[] }) => {
      this.tabuWordDetected$.next(data);
    });

    this.hubConnection.on('MatchmakingError', (msg: string) => {
      this.matchmakingError$.next(msg);
    });

    this.hubConnection.on('RoomNotFound', (msg: string) => {
      this.roomNotFound$.next(msg);
    });

    this.hubConnection.on('MatchmakingCancelled', () => {
      // handled locally
    });

    this.hubConnection.on('OpponentDisconnected', (data: { disconnectedPlayerId: string; winnerId: string | null }) => {
      this.opponentDisconnected$.next(data);
    });

    this.hubConnection.onreconnecting(() => {
      this.connectionState$.next('reconnecting');
    });

    this.hubConnection.onreconnected(() => {
      this.connectionState$.next('connected');
    });

    this.hubConnection.onclose(() => {
      this.connectionState$.next('disconnected');
    });
  }

  // ---- SignalR Actions ----

  async joinMatchmaking(category?: string, difficulty?: number): Promise<void> {
    await this.ensureConnected();
    await this.hubConnection!.invoke('JoinMatchmaking', category ?? null, difficulty ?? null);
  }

  async leaveMatchmaking(): Promise<void> {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      await this.hubConnection.invoke('LeaveMatchmaking');
    }
  }

  async joinRoom(roomCode: string): Promise<void> {
    await this.ensureConnected();
    await this.hubConnection!.invoke('JoinRoom', roomCode);
  }

  async waitInRoom(roomCode: string): Promise<void> {
    await this.ensureConnected();
    await this.hubConnection!.invoke('WaitInRoom', roomCode);
  }

  async submitVersusPrompt(versusGameId: string, gameSessionId: string, prompt: string): Promise<void> {
    await this.ensureConnected();
    await this.hubConnection!.invoke('SubmitVersusPrompt', versusGameId, gameSessionId, prompt);
  }

  private async ensureConnected(): Promise<void> {
    if (this.hubConnection?.state !== signalR.HubConnectionState.Connected) {
      await this.startConnection();
    }
  }

  // ---- REST API ----

  createRoom(request: CreateRoomRequest): Observable<CreateRoomResponse> {
    return this.http.post<CreateRoomResponse>(`${this.apiUrl}/create-room`, request);
  }

  getRoom(roomCode: string): Observable<VersusGame> {
    return this.http.get<VersusGame>(`${this.apiUrl}/room/${roomCode}`);
  }

  getHistory(userId: string, page = 1, pageSize = 10): Observable<VersusGame[]> {
    return this.http.get<VersusGame[]>(`${this.apiUrl}/history/${userId}`, {
      params: { page: page.toString(), pageSize: pageSize.toString() }
    });
  }

  ngOnDestroy(): void {
    this.stopConnection();
  }
}
