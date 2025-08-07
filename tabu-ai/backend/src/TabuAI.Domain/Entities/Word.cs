namespace TabuAI.Domain.Entities;

public class Word
{
    public Guid Id { get; set; }
    public string TargetWord { get; set; } = string.Empty;
    public List<string> TabuWords { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}

public enum DifficultyLevel
{
    Easy = 1,
    Medium = 2,
    Hard = 3,
    Expert = 4
}