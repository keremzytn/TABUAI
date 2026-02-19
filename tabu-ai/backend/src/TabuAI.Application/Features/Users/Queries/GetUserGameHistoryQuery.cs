using MediatR;
using TabuAI.Application.Features.Users.DTOs;

namespace TabuAI.Application.Features.Users.Queries;

public class GetUserGameHistoryQuery : IRequest<List<GameHistoryDto>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetUserGameHistoryQuery(Guid userId, int page = 1, int pageSize = 10)
    {
        UserId = userId;
        Page = page;
        PageSize = pageSize;
    }
}
