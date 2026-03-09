using MediatR;

namespace TabuAI.Application.Features.Admin.Queries;

public record GetAllWordPacksQuery : IRequest<IEnumerable<WordPackDto>>;

public class WordPackDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string CreatedByUsername { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public bool IsApproved { get; set; }
    public int PlayCount { get; set; }
    public int LikeCount { get; set; }
    public int WordCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
