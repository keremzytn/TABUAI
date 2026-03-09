using TabuAI.Domain.Enums;

namespace TabuAI.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public PlayerLevel Level { get; set; } = PlayerLevel.Rookie;
    public UserRole Role { get; set; } = UserRole.User;
    public int TotalScore { get; set; } = 0;
    public int GamesPlayed { get; set; } = 0;
    public int GamesWon { get; set; } = 0;
    public decimal WinRate => GamesPlayed > 0 ? (decimal)GamesWon / GamesPlayed * 100 : 0;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string PasswordHash { get; set; } = string.Empty;
    public string? GoogleId { get; set; }
    public string? FacebookId { get; set; }
    public string? MicrosoftId { get; set; }
    
    public int PromptCoins { get; set; } = 0;
    public int CurrentStreak { get; set; } = 0;
    public int BestStreak { get; set; } = 0;
    public DateTime? LastPlayedAt { get; set; }
    
    // Navigation properties
    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    public ICollection<UserStatistic> UserStatistics { get; set; } = new List<UserStatistic>();
    public ICollection<Friendship> SentFriendRequests { get; set; } = new List<Friendship>();
    public ICollection<Friendship> ReceivedFriendRequests { get; set; } = new List<Friendship>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}

public enum PlayerLevel
{
    Rookie = 1,          // Çaylak
    Apprentice = 2,      // Çırak
    Skilled = 3,         // Becerikli
    Expert = 4,          // Uzman
    Master = 5,          // Usta
    GrandMaster = 6      // Usta Promptçı
}