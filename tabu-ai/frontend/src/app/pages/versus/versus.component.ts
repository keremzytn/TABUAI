import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { VersusService } from '../../services/versus.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import {
  VersusGameStartedEvent,
  PlayerAttemptResultEvent,
  GameCompletedEvent,
  VersusPlayer
} from '../../models/versus.models';
import { Word } from '../../models/game.models';

@Component({
  selector: 'app-versus',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './versus.component.html',
  styleUrl: './versus.component.scss'
})
export class VersusComponent implements OnInit, OnDestroy {
  // State
  phase: 'lobby' | 'waiting' | 'playing' | 'finished' = 'lobby';
  lobbyMode: 'menu' | 'matchmaking' | 'create-room' | 'join-room' = 'menu';

  // Room
  roomCode = '';
  joinRoomCode = '';
  createdRoomCode = '';

  // Game
  versusGameId = '';
  word: Word | null = null;
  currentPrompt = '';
  submitting = false;
  detectedTabuWord: string | null = null;

  // Players
  myPlayer: VersusPlayer | null = null;
  opponentPlayer: VersusPlayer | null = null;
  myGameSessionId = '';

  // Attempt tracking
  myAttempts: AttemptInfo[] = [];
  opponentAttempts: AttemptInfo[] = [];
  myFinished = false;
  opponentFinished = false;

  // Result
  winnerId: string | null = null;
  isDraw = false;

  // Timer
  gameTime = '00:00';
  private timerInterval: any = null;
  private startTime: Date | null = null;

  // Category
  selectedCategory: string | null = null;
  categories = ['Ula\u015F\u0131m', 'Teknoloji', 'Bilim', 'Sanat', 'Yemek', 'Spor', 'Tarih', 'Do\u011Fa', 'M\u00FCzik'];

  private subs: Subscription[] = [];

  constructor(
    private versusService: VersusService,
    private authService: AuthService,
    private toastService: ToastService,
    private router: Router
  ) {}

  get userId(): string {
    return this.authService.currentUserValue?.id ?? '';
  }

  get isLoggedIn(): boolean {
    return !!this.authService.currentUserValue;
  }

  ngOnInit(): void {
    if (!this.isLoggedIn) {
      this.toastService.warning('D\u00FCello modu i\u00E7in giri\u015F yapmal\u0131s\u0131n\u0131z.');
      this.router.navigate(['/login']);
      return;
    }

    this.setupSignalRListeners();
  }

  ngOnDestroy(): void {
    this.subs.forEach(s => s.unsubscribe());
    this.stopTimer();
    this.versusService.stopConnection();
  }

  private setupSignalRListeners(): void {
    this.subs.push(
      this.versusService.onWaitingForOpponent.subscribe(() => {
        this.phase = 'waiting';
        this.toastService.info('Rakip aran\u0131yor...');
      }),

      this.versusService.onGameStarted.subscribe((data: VersusGameStartedEvent) => {
        this.handleGameStarted(data);
      }),

      this.versusService.onPlayerAttemptResult.subscribe((data: PlayerAttemptResultEvent) => {
        this.handleAttemptResult(data);
      }),

      this.versusService.onGameCompleted.subscribe((data: GameCompletedEvent) => {
        this.handleGameCompleted(data);
      }),

      this.versusService.onTabuWordDetected.subscribe((data) => {
        this.toastService.warning(`Tabu kelimeler kulland\u0131n\u0131z: ${data.detectedWords.join(', ')}`);
      }),

      this.versusService.onMatchmakingError.subscribe((msg: string) => {
        this.toastService.error(msg);
        this.phase = 'lobby';
        this.lobbyMode = 'menu';
      }),

      this.versusService.onRoomNotFound.subscribe((msg: string) => {
        this.toastService.error(msg);
      }),

      this.versusService.onOpponentDisconnected.subscribe((data) => {
        this.handleOpponentDisconnected(data);
      })
    );
  }

  // ---- Lobby Actions ----

  async startMatchmaking(): Promise<void> {
    this.lobbyMode = 'matchmaking';
    this.phase = 'waiting';
    await this.versusService.joinMatchmaking(this.selectedCategory ?? undefined);
  }

  async cancelMatchmaking(): Promise<void> {
    await this.versusService.leaveMatchmaking();
    this.phase = 'lobby';
    this.lobbyMode = 'menu';
  }

  async createRoom(): Promise<void> {
    this.versusService.createRoom({
      userId: this.userId,
      category: this.selectedCategory ?? undefined
    }).subscribe({
      next: async (response) => {
        this.createdRoomCode = response.roomCode;
        this.versusGameId = response.versusGameId;
        this.lobbyMode = 'create-room';
        this.phase = 'waiting';
        this.toastService.success(`Oda olu\u015Fturuldu: ${response.roomCode}`);
        await this.versusService.waitInRoom(response.roomCode);
      },
      error: () => {
        this.toastService.error('Oda olu\u015Fturulamad\u0131.');
      }
    });
  }

  async joinRoom(): Promise<void> {
    if (!this.joinRoomCode.trim()) return;
    this.phase = 'waiting';
    await this.versusService.joinRoom(this.joinRoomCode.toUpperCase());
  }

  // ---- Game Logic ----

  private handleGameStarted(data: VersusGameStartedEvent): void {
    this.versusGameId = data.versusGameId;
    this.roomCode = data.roomCode;
    this.word = data.word;
    this.phase = 'playing';

    const isPlayer1 = data.player1.id === this.userId;
    this.myPlayer = isPlayer1 ? data.player1 : data.player2;
    this.opponentPlayer = isPlayer1 ? data.player2 : data.player1;
    this.myGameSessionId = this.myPlayer.gameSessionId ?? '';

    this.myAttempts = [];
    this.opponentAttempts = [];
    this.myFinished = false;
    this.opponentFinished = false;

    this.startTimer();
    this.toastService.success('D\u00FCello ba\u015Flad\u0131!');
  }

  private handleAttemptResult(data: PlayerAttemptResultEvent): void {
    const isMe = data.playerId === this.userId;
    const attemptInfo: AttemptInfo = {
      attemptNumber: data.attemptNumber,
      prompt: data.prompt,
      aiGuess: data.aiGuess,
      isCorrect: data.isCorrect,
      score: data.score
    };

    if (isMe) {
      this.myAttempts.push(attemptInfo);
      this.submitting = false;
      this.currentPrompt = '';

      if (this.myPlayer) {
        this.myPlayer.score = data.totalScore;
        this.myPlayer.attempts = data.attemptNumber;
      }

      if (data.playerFinished) {
        this.myFinished = true;
        if (data.isCorrect) {
          this.toastService.success(`AI bildi! +${data.score} puan`);
        } else {
          this.toastService.warning('3 hakk\u0131n\u0131z doldu.');
        }
      } else if (!data.isCorrect) {
        this.toastService.info('AI bilemedi. Tekrar deneyin!');
      }
    } else {
      this.opponentAttempts.push(attemptInfo);

      if (this.opponentPlayer) {
        this.opponentPlayer.score = data.totalScore;
        this.opponentPlayer.attempts = data.attemptNumber;
      }

      if (data.playerFinished) {
        this.opponentFinished = true;
        this.toastService.info('Rakibiniz tamamlad\u0131!');
      }
    }
  }

  private handleGameCompleted(data: GameCompletedEvent): void {
    this.phase = 'finished';
    this.winnerId = data.winnerId ?? null;
    this.isDraw = data.isDraw;
    this.stopTimer();

    // Update final scores
    const isPlayer1 = data.player1.id === this.userId;
    this.myPlayer = isPlayer1 ? data.player1 : data.player2;
    this.opponentPlayer = isPlayer1 ? data.player2 : data.player1;

    if (this.isDraw) {
      this.toastService.info('D\u00FCello berabere bitti!');
    } else if (this.winnerId === this.userId) {
      this.toastService.success('Tebrikler, d\u00FCelloyu kazand\u0131n\u0131z!');
    } else {
      this.toastService.warning('D\u00FCelloyu kaybettiniz.');
    }
  }

  private handleOpponentDisconnected(data: { disconnectedPlayerId: string; winnerId: string | null }): void {
    this.phase = 'finished';
    this.winnerId = data.winnerId;
    this.isDraw = false;
    this.stopTimer();

    if (data.winnerId === this.userId) {
      this.toastService.success('Rakibiniz bağlantısını kesti. Düelloyu kazandınız!');
    } else {
      this.toastService.warning('Rakibiniz bağlantısını kesti.');
    }
  }

  submitPrompt(): void {
    if (!this.currentPrompt.trim() || this.submitting || this.myFinished) return;

    if (this.detectedTabuWord) {
      this.toastService.warning(`"${this.detectedTabuWord}" yasakl\u0131 bir kelime!`);
      return;
    }

    this.submitting = true;
    this.versusService.submitVersusPrompt(this.versusGameId, this.myGameSessionId, this.currentPrompt);
  }

  checkTabuWords(): void {
    if (!this.word) return;
    this.detectedTabuWord = null;

    const wordsInPrompt = this.currentPrompt.toLowerCase()
      .replace(/[.,!?;:()"]/g, ' ')
      .split(/\s+/)
      .filter(w => w.length > 0);

    for (const tabu of this.word.tabuWords) {
      const tabuLower = tabu.toLowerCase();
      for (const w of wordsInPrompt) {
        if (w === tabuLower) {
          this.detectedTabuWord = tabu;
          return;
        }
        if (w.startsWith(tabuLower) && tabuLower.length > 3) {
          const suffix = w.substring(tabuLower.length);
          if (suffix.length <= 3) {
            this.detectedTabuWord = tabu;
            return;
          }
        }
      }
    }
  }

  onEnter(event: Event): void {
    const keyboardEvent = event as KeyboardEvent;
    if (!keyboardEvent.shiftKey) {
      event.preventDefault();
      this.submitPrompt();
    }
  }

  getDifficultyText(level: number): string {
    const levels: Record<number, string> = { 1: 'KOLAY', 2: 'ORTA', 3: 'ZOR', 4: 'UZMAN' };
    return levels[level] || '???';
  }

  copyRoomCode(): void {
    navigator.clipboard.writeText(this.createdRoomCode);
    this.toastService.success('Oda kodu kopyaland\u0131!');
  }

  backToLobby(): void {
    this.phase = 'lobby';
    this.lobbyMode = 'menu';
    this.word = null;
    this.myAttempts = [];
    this.opponentAttempts = [];
    this.myFinished = false;
    this.opponentFinished = false;
    this.currentPrompt = '';
    this.stopTimer();
  }

  // Timer
  private startTimer(): void {
    this.startTime = new Date();
    this.timerInterval = setInterval(() => {
      if (!this.startTime) return;
      const elapsed = Math.floor((Date.now() - this.startTime.getTime()) / 1000);
      const m = Math.floor(elapsed / 60);
      const s = elapsed % 60;
      this.gameTime = `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`;
    }, 1000);
  }

  private stopTimer(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }
  }
}

interface AttemptInfo {
  attemptNumber: number;
  prompt: string;
  aiGuess: string;
  isCorrect: boolean;
  score: number;
}
