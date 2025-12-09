export interface Word {
  id: string;
  targetWord: string;
  tabuWords: string[];
  category: string;
  difficulty: number;
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
}

export interface StartGameRequest {
  userId: string;  // Will be converted to Guid on backend
  gameMode: string;
  category?: string;
  difficulty?: number;
}

export interface SubmitPromptRequest {
  gameSessionId: string;  // Will be converted to Guid on backend
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