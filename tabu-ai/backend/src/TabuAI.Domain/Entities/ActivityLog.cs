namespace TabuAI.Domain.Entities;

public class ActivityLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ActivityType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? ScoreEarned { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}

public enum ActivityType
{
    GameWon = 1,
    GameLost = 2,
    VersusWon = 3,
    VersusLost = 4,
    VersusDraw = 5,
    ChallengeReceived = 6,
    ChallengeSent = 7,
    BadgeEarned = 8,
    LevelUp = 9
}
