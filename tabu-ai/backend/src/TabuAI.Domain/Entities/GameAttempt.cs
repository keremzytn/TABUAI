using System.Text.Json.Serialization;

namespace TabuAI.Domain.Entities;

public class GameAttempt
{
    public Guid Id { get; set; }
    public Guid GameSessionId { get; set; }
    public int AttemptNumber { get; set; }
    public string UserPrompt { get; set; } = string.Empty;
    public string AiGuess { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int Score { get; set; }
    public string? AiFeedback { get; set; }
    public PromptQuality? PromptQuality { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [JsonIgnore]
    public GameSession GameSession { get; set; } = null!;
}
