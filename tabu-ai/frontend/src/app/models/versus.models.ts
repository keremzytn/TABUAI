import { Word } from './game.models';

export interface VersusPlayer {
  id: string;
  displayName: string;
  score: number;
  attempts: number;
  gameSessionId?: string;
}

export interface VersusGame {
  id: string;
  roomCode: string;
  word?: Word;
  player1: VersusPlayer;
  player2?: VersusPlayer;
  status: string;
  winnerId?: string;
  createdAt: string;
  startedAt?: string;
  completedAt?: string;
}

export interface CreateRoomRequest {
  userId: string;
  category?: string;
  difficulty?: number;
}

export interface CreateRoomResponse {
  versusGameId: string;
  roomCode: string;
}

export interface VersusGameStartedEvent {
  versusGameId: string;
  roomCode: string;
  word: Word;
  player1: VersusPlayer;
  player2: VersusPlayer;
}

export interface PlayerAttemptResultEvent {
  playerId: string;
  attemptNumber: number;
  isCorrect: boolean;
  aiGuess: string;
  score: number;
  totalScore: number;
  playerFinished: boolean;
  prompt: string;
}

export interface GameCompletedEvent {
  versusGameId: string;
  winnerId?: string;
  isDraw: boolean;
  player1: VersusPlayer;
  player2: VersusPlayer;
}

export interface Challenge {
  id: string;
  challenger: ChallengeUser;
  challenged: ChallengeUser;
  word: Word;
  status: string;
  message?: string;
  versusGameId?: string;
  createdAt: string;
  expiresAt: string;
}

export interface ChallengeUser {
  id: string;
  username?: string;
  displayName?: string;
}

export interface SendChallengeRequest {
  challengerId: string;
  challengedId: string;
  wordId?: string;
  category?: string;
  message?: string;
}

export interface ActivityLog {
  id: string;
  userId: string;
  userDisplayName: string;
  type: string;
  description: string;
  scoreEarned?: number;
  createdAt: string;
}
