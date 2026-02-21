import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { GameSession, StartGameRequest, SubmitPromptRequest, GameResult } from '../models/game.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private readonly baseUrl = environment.apiUrl;

  private currentGameSubject = new BehaviorSubject<GameSession | null>(null);
  public currentGame$ = this.currentGameSubject.asObservable();

  private gameStateSubject = new BehaviorSubject<'idle' | 'playing' | 'completed'>('idle');
  public gameState$ = this.gameStateSubject.asObservable();

  constructor(private http: HttpClient) { }

  startGame(request: StartGameRequest): Observable<GameSession> {
    return this.http.post<GameSession>(`${this.baseUrl}/game/start`, request);
  }

  submitPrompt(request: SubmitPromptRequest): Observable<GameResult> {
    return this.http.post<GameResult>(`${this.baseUrl}/game/submit-prompt`, request);
  }

  getGameSession(gameSessionId: string): Observable<GameSession> {
    return this.http.get<GameSession>(`${this.baseUrl}/game/${gameSessionId}`);
  }

  setCurrentGame(game: GameSession | null): void {
    // Swap order to ensure gameState is 'playing' BEFORE currentGame$ notifies
    this.gameStateSubject.next(game ? 'playing' : 'idle');
    this.currentGameSubject.next(game);
  }

  completeGame(): void {
    this.gameStateSubject.next('completed');
  }

  getCurrentGame(): GameSession | null {
    return this.currentGameSubject.value;
  }

  // Demo mode için mock veri
  startDemoGame(): Observable<GameSession> {
    const mockGameSession: GameSession = {
      id: 'demo-game-1',
      userId: 'demo-user',
      word: {
        id: 'word-1',
        targetWord: 'Uçak',
        tabuWords: ['hava', 'kanat', 'yolcu', 'pilot', 'uçmak'],
        category: 'Ulaşım',
        difficulty: 1
      },
      userPrompt: '',
      isCorrectGuess: false,
      score: 0,
      timeSpent: '00:00:00',
      attemptNumber: 1,
      startedAt: new Date().toISOString(),
      status: 'InProgress',
      suggestions: [],
      attempts: []
    };

    return new Observable(observer => {
      setTimeout(() => {
        observer.next(mockGameSession);
        observer.complete();
      }, 1000);
    });
  }

  // Demo mode için mock prompt submission
  submitDemoPrompt(prompt: string, targetWord: string, tabuWords: string[]): Observable<GameResult> {
    const containsTabu = tabuWords.some(tabu =>
      prompt.toLowerCase().includes(tabu.toLowerCase())
    );

    const isCorrect = !containsTabu && Math.random() > 0.3; // %70 doğru tahmin şansı

    const mockResult: GameResult = {
      isCorrect,
      aiGuess: isCorrect ? targetWord : 'Araba',
      aiFeedback: containsTabu
        ? 'Tabu kelimeler kullandınız!'
        : isCorrect
          ? 'Harika! Mükemmel bir tanımlama yaptınız.'
          : 'İyi bir deneme, ancak daha spesifik olabilirsiniz.',
      score: isCorrect ? Math.floor(Math.random() * 50) + 50 : 0,
      promptQuality: containsTabu ? 1 : Math.floor(Math.random() * 3) + 2,
      suggestions: isCorrect
        ? ['Tebrikler! Başarılı bir prompt yazdınız.', 'Farklı açılardan yaklaşmayı deneyin.']
        : ['Daha spesifik detaylar ekleyin.', 'Nesnenin kullanım alanlarını belirtin.'],
      gameCompleted: isCorrect,
      history: [] // Simplified for demo
    };

    return new Observable(observer => {
      setTimeout(() => {
        observer.next(mockResult);
        observer.complete();
      }, 2000); // AI düşünme simülasyonu
    });
  }
}