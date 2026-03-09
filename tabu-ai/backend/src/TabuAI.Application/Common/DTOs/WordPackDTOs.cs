namespace TabuAI.Application.Common.DTOs;

public class WordPackDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "tr";
    public string CreatedByUsername { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public bool IsPublic { get; set; }
    public bool IsApproved { get; set; }
    public int PlayCount { get; set; }
    public int LikeCount { get; set; }
    public int WordCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<WordDto> Words { get; set; } = new();
}

public class CreateWordPackRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "tr";
    public bool IsPublic { get; set; } = true;
    public List<CreateWordInPackRequest> Words { get; set; } = new();
}

public class CreateWordInPackRequest
{
    public string TargetWord { get; set; } = string.Empty;
    public List<string> TabuWords { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public int Difficulty { get; set; } = 1;
}

public class DailyChallengeDto
{
    public Guid Id { get; set; }
    public DateTime ChallengeDate { get; set; }
    public string Language { get; set; } = "tr";
    public WordDto Word { get; set; } = null!;
    public bool AlreadyPlayed { get; set; }
    public int TotalPlayers { get; set; }
}

public class DailyChallengeLeaderboardDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int AttemptsUsed { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public int Rank { get; set; }
}

public class DailyChallengeResultDto
{
    public int Score { get; set; }
    public int Rank { get; set; }
    public int TotalPlayers { get; set; }
    public List<DailyChallengeLeaderboardDto> TopPlayers { get; set; } = new();
}

public class CompleteDailyChallengeRequest
{
    public string UserId { get; set; } = string.Empty;
    public string DailyChallengeId { get; set; } = string.Empty;
    public string GameSessionId { get; set; } = string.Empty;
}
