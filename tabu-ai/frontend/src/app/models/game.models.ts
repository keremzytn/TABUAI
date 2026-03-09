export interface Word {
  id: string;
  targetWord: string;
  tabuWords: string[];
  category: string;
  difficulty: number;
  language?: string;
}

export type GameLanguage = 'tr' | 'en' | 'de' | 'fr';

export interface LanguageInfo {
  id: GameLanguage;
  name: string;
  flag: string;
}

export const SUPPORTED_LANGUAGES: LanguageInfo[] = [
  { id: 'tr', name: 'Türkçe', flag: '🇹🇷' },
  { id: 'en', name: 'English', flag: '🇬🇧' },
  { id: 'de', name: 'Deutsch', flag: '🇩🇪' },
  { id: 'fr', name: 'Français', flag: '🇫🇷' }
];

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
  language?: string;
}

export interface SubmitPromptRequest {
  gameSessionId: string;
  prompt: string;
  persona?: string;
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
  aiReaction?: string;
  promptCoach?: PromptCoachResult;
}

export interface PromptCoachResult {
  overallAnalysis: string;
  bestPrompt: string;
  idealPromptExample: string;
  tipsForNextTime: string[];
  promptEngineeringScore: number;
}

export type AiPersona = 'default' | 'sarcastic' | 'childish' | 'meticulous' | 'dramatic' | 'philosopher';

export interface PersonaInfo {
  id: AiPersona;
  name: string;
  icon: string;
  description: string;
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

// Word Pack (UGC)
export interface WordPack {
  id: string;
  name: string;
  description: string;
  language: string;
  createdByUsername: string;
  createdByUserId: string;
  isPublic: boolean;
  isApproved: boolean;
  playCount: number;
  likeCount: number;
  wordCount: number;
  createdAt: string;
  words?: Word[];
}

export interface CreateWordPackRequest {
  name: string;
  description: string;
  language: string;
  isPublic: boolean;
  words: CreateWordInPackRequest[];
}

export interface CreateWordInPackRequest {
  targetWord: string;
  tabuWords: string[];
  category: string;
  difficulty: number;
}

// Daily Challenge
export interface DailyChallenge {
  id: string;
  challengeDate: string;
  language: string;
  word: Word;
  alreadyPlayed: boolean;
  totalPlayers: number;
}

export interface DailyChallengeLeaderboard {
  userId: string;
  displayName: string;
  score: number;
  attemptsUsed: number;
  timeTaken: string;
  rank: number;
}

export interface DailyChallengeResult {
  score: number;
  rank: number;
  totalPlayers: number;
  topPlayers: DailyChallengeLeaderboard[];
}