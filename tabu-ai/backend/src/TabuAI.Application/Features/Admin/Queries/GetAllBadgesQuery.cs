using MediatR;

namespace TabuAI.Application.Features.Admin.Queries;

public record GetAllBadgesQuery : IRequest<IEnumerable<BadgeDto>>;

public class BadgeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int RequiredValue { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserCount { get; set; }
}
