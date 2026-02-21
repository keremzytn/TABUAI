export interface Friend {
    userId: string;
    username: string;
    displayName?: string;
    level: string;
    totalScore: number;
    friendSince: Date;
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
