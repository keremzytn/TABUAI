namespace TabuAI.Domain.Entities;

public class DailyChallenge
{
    public Guid Id { get; set; }
    public DateTime ChallengeDate { get; set; }
    public Guid WordId { get; set; }
    public string Language { get; set; } = "tr";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Word Word { get; set; } = null!;
}

public class DailyChallengeEntry
{
    public Guid Id { get; set; }
    public Guid DailyChallengeId { get; set; }
    public Guid UserId { get; set; }
    public Guid GameSessionId { get; set; }
    public int Score { get; set; }
    public int AttemptsUsed { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public DailyChallenge DailyChallenge { get; set; } = null!;
    public User User { get; set; } = null!;
    public GameSession GameSession { get; set; } = null!;
}
