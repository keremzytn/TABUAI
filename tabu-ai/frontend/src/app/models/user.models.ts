export interface UserProfile {
    id: string;
    username: string;
    email: string;
    displayName?: string;
    level: string;
    role: string;
    totalScore: number;
    gamesPlayed: number;
    gamesWon: number;
    winRate: number;
    isActive: boolean;
    createdAt: Date;
    badges: UserBadge[];
    promptCoins: number;
    currentStreak: number;
    bestStreak: number;
    selectedAvatar?: string | null;
    selectedCardDesign?: string | null;
}

export interface UserBadge {
    badgeId: string;
    name: string;
    description: string;
    iconUrl?: string;
    earnedAt: Date;
}

export interface UserStatistic {
    metricName: string;
    value: number;
    type: number;
    formattedValue: string;
}

export interface GameHistory {
    id: string;
    targetWord: string;
    score: number;
    isWin: boolean;
    timeSpent: string; // TimeSpan comes as string in JSON usually "00:00:00"
    playedAt: Date;
    mode: number;
    attemptNumber: number;
}

export interface PromptAnalysisChart {
    dataPoints: PromptAnalysisDataPoint[];
}

export interface PromptAnalysisDataPoint {
    date: string;
    averagePromptLength: number;
    successRate: number;
    uniqueWordsUsed: number;
    gamesPlayed: number;
    averageScore: number;
}

export interface StyleAnalysis {
    playerTitle: string;
    titleEmoji: string;
    description: string;
    traits: StyleTrait[];
    promptStyle: PromptStyle;
    topCategories: CategoryPerformance[];
}

export interface StyleTrait {
    name: string;
    emoji: string;
    value: number;
    description: string;
}

export interface PromptStyle {
    averageLength: number;
    adjectiveRatio: number;
    verbRatio: number;
    uniqueWordRatio: number;
    dominantStyle: string;
}

export interface CategoryPerformance {
    category: string;
    gamesPlayed: number;
    successRate: number;
    averageScore: number;
}

export interface BadgeGallery {
    earnedBadges: BadgeShowcase[];
    lockedBadges: BadgeShowcase[];
    showcaseBadgeIds: string[];
    totalBadges: number;
    earnedCount: number;
    completionPercentage: number;
}

export interface BadgeShowcase {
    badgeId: string;
    name: string;
    description: string;
    iconUrl: string;
    badgeType: string;
    requiredValue: number;
    currentProgress: number;
    progressPercentage: number;
    earnedAt?: string;
    isEarned: boolean;
    rarity: string;
}
