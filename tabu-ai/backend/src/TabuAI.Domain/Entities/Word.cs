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
    public string Language { get; set; } = "tr";
    public Guid? CreatedByUserId { get; set; }
    public Guid? WordPackId { get; set; }
    
    // Navigation properties
    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    public User? CreatedByUser { get; set; }
    public WordPack? WordPack { get; set; }
}

public enum DifficultyLevel
{
    Easy = 1,
    Medium = 2,
    Hard = 3,
    Expert = 4
}