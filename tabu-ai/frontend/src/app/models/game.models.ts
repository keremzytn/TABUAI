export interface Word {
  id: string;
  targetWord: string;
  tabuWords: string[];
  category: string;
  difficulty: number;
}

export interface GameAttempt {
  attemptNumber: number;
  userPrompt: string;
  aiGuess: string;
  isCorrect: boolean;
  score: number;
  aiFeedback?: string;
  promptQuality?: number;
  createdAt: string;
}

export interface GameSession {
  id: string;
  userId: string;
  word: Word;
  userPrompt: string;
  aiResponse?: string;
  isCorrectGuess: boolean;
  score: number;
  timeSpent: string;
  attemptNumber: number;
  startedAt: string;
  completedAt?: string;
  status: string;
  aiFeedback?: string;
  promptQuality?: number;
  suggestions: string[];
  attempts: GameAttempt[];
}

export interface StartGameRequest {
  userId: string;
  gameMode: string;
  category?: string;
  difficulty?: number;
}

export interface SubmitPromptRequest {
  gameSessionId: string;
  prompt: string;
}

export interface GameResult {
  isCorrect: boolean;
  aiGuess: string;
  aiFeedback: string;
  score: number;
  promptQuality: number;
  suggestions: string[];
  gameCompleted: boolean;
  history: GameAttempt[];
}

export interface User {
  id: string;
  username: string;
  displayName?: string;
  level: string;
  totalScore: number;
  gamesPlayed: number;
  gamesWon: number;
  winRate: number;
  createdAt: string;
}

export interface ApiResponse<T> {
  data?: T;
  message?: string;
  error?: string;
}