using MediatR;

namespace TabuAI.Application.Features.Admin.Queries;

public record GetActivityLogsQuery(int Page = 1, int Limit = 100) : IRequest<PagedResultDto<ActivityLogAdminDto>>;

public class ActivityLogAdminDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? ScoreEarned { get; set; }
    public DateTime CreatedAt { get; set; }
}
