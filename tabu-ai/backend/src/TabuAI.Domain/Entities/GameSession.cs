namespace TabuAI.Domain.Entities;

public class GameSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WordId { get; set; }
    public GameMode Mode { get; set; }
    public string UserPrompt { get; set; } = string.Empty;
    public string? AiResponse { get; set; }
    public bool IsCorrectGuess { get; set; }
    public int Score { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public GameStatus Status { get; set; } = GameStatus.InProgress;
    
    // AI Feedback
    public string? AiFeedback { get; set; }
    public PromptQuality? PromptQuality { get; set; }
    public List<string> Suggestions { get; set; } = new();
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Word Word { get; set; } = null!;
}

public enum GameMode
{
    Solo = 1,
    Multiplayer = 2,
    DailyChallenge = 3,
    Tournament = 4
}

public enum GameStatus
{
    InProgress = 1,
    Completed = 2,
    Abandoned = 3,
    TimeOut = 4,
    Failed = 5
}

public enum PromptQuality
{
    Poor = 1,
    Fair = 2,
    Good = 3,
    Excellent = 4,
    Perfect = 5
}