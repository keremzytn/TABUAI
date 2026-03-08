namespace TabuAI.Domain.Entities;

public class VersusGame
{
    public Guid Id { get; set; }
    public Guid WordId { get; set; }
    public Guid Player1Id { get; set; }
    public Guid? Player2Id { get; set; }
    public Guid? Player1GameSessionId { get; set; }
    public Guid? Player2GameSessionId { get; set; }
    public VersusGameStatus Status { get; set; } = VersusGameStatus.WaitingForOpponent;
    public Guid? WinnerId { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
    public int Player1Attempts { get; set; }
    public int Player2Attempts { get; set; }
    public string RoomCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public Word Word { get; set; } = null!;
    public User Player1 { get; set; } = null!;
    public User? Player2 { get; set; }
    public GameSession? Player1GameSession { get; set; }
    public GameSession? Player2GameSession { get; set; }
}

public enum VersusGameStatus
{
    WaitingForOpponent = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Expired = 5
}
