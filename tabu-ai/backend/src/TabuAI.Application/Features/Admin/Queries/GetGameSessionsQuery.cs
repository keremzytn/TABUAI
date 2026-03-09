using MediatR;

namespace TabuAI.Application.Features.Admin.Queries;

public record GetGameSessionsQuery(int Page = 1, int Limit = 50) : IRequest<PagedResultDto<GameSessionAdminDto>>;

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
}

public class GameSessionAdminDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string TargetWord { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsCorrectGuess { get; set; }
    public int Score { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
