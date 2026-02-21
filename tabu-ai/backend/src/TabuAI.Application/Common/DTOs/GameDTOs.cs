namespace TabuAI.Application.Common.DTOs;

public class WordDto
{
    public Guid Id { get; set; }
    public string TargetWord { get; set; } = string.Empty;
    public List<string> TabuWords { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public int Difficulty { get; set; }
}

public class GameAttemptDto
{
    public int AttemptNumber { get; set; }
    public string UserPrompt { get; set; } = string.Empty;
    public string AiGuess { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int Score { get; set; }
    public string? AiFeedback { get; set; }
    public int? PromptQuality { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GameSessionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public WordDto Word { get; set; } = null!;
    public string UserPrompt { get; set; } = string.Empty;
    public string? AiResponse { get; set; }
    public bool IsCorrectGuess { get; set; }
    public int Score { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AiFeedback { get; set; }
    public int? PromptQuality { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public List<GameAttemptDto> Attempts { get; set; } = new();
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Level { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public decimal WinRate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StartGameRequest
{
    public string UserId { get; set; } = string.Empty;
    public string GameMode { get; set; } = "Solo";
    public string? Category { get; set; }
    public int? Difficulty { get; set; }
}

public class SubmitPromptRequest
{
    public string GameSessionId { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
}

public class GameResultDto
{
    public bool IsCorrect { get; set; }
    public string AiGuess { get; set; } = string.Empty;
    public string AiFeedback { get; set; } = string.Empty;
    public int Score { get; set; }
    public int PromptQuality { get; set; }
    public List<string> Suggestions { get; set; } = new();
    public bool GameCompleted { get; set; }
    public List<GameAttemptDto> History { get; set; } = new();
}