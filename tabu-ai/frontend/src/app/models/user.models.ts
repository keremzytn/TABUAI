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
    createdAt: Date;
    badges: UserBadge[];
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
