namespace TabuAI.Application.Common.DTOs;

public class VersusGameDto
{
    public Guid Id { get; set; }
    public string RoomCode { get; set; } = string.Empty;
    public WordDto? Word { get; set; }
    public VersusPlayerDto Player1 { get; set; } = null!;
    public VersusPlayerDto? Player2 { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? WinnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class VersusPlayerDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int Attempts { get; set; }
    public Guid? GameSessionId { get; set; }
}

public class CreateRoomRequest
{
    public string UserId { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int? Difficulty { get; set; }
}

public class CreateRoomResponse
{
    public Guid VersusGameId { get; set; }
    public string RoomCode { get; set; } = string.Empty;
}

public class ChallengeDto
{
    public Guid Id { get; set; }
    public UserDto Challenger { get; set; } = null!;
    public UserDto Challenged { get; set; } = null!;
    public WordDto Word { get; set; } = null!;
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public Guid? VersusGameId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class SendChallengeRequest
{
    public string ChallengerId { get; set; } = string.Empty;
    public string ChallengedId { get; set; } = string.Empty;
    public string? WordId { get; set; }
    public string? Category { get; set; }
    public string? Message { get; set; }
}

public class RespondChallengeRequest
{
    public string UserId { get; set; } = string.Empty;
    public bool Accept { get; set; }
}

public class ActivityLogDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserDisplayName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ScoreEarned { get; set; }
    public DateTime CreatedAt { get; set; }
}
