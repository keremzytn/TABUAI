namespace TabuAI.Domain.Entities;

public class Challenge
{
    public Guid Id { get; set; }
    public Guid ChallengerId { get; set; }
    public Guid ChallengedId { get; set; }
    public Guid WordId { get; set; }
    public Guid? VersusGameId { get; set; }
    public ChallengeStatus Status { get; set; } = ChallengeStatus.Pending;
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    // Navigation properties
    public User Challenger { get; set; } = null!;
    public User Challenged { get; set; } = null!;
    public Word Word { get; set; } = null!;
    public VersusGame? VersusGame { get; set; }
}

public enum ChallengeStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2,
    Expired = 3
}
