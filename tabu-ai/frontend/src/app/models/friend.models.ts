export interface Friend {
    friendshipId: string;
    userId: string;
    username: string;
    displayName?: string;
    level: string;
    totalScore: number;
    gamesPlayed: number;
    gamesWon: number;
    winRate: number;
    friendSince: Date;
    createdAt: Date;
}

export interface FriendRequest {
    requestId: string;
    fromUserId: string;
    fromUsername: string;
    fromDisplayName?: string;
    sentAt: Date;
}

export interface UserSearchResult {
    userId: string;
    username: string;
    displayName?: string;
    level: string;
    friendshipStatus?: string; // null, 'Pending', 'Accepted'
}
