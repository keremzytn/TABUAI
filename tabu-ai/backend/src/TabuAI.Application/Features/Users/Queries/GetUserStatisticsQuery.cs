using MediatR;
using TabuAI.Application.Features.Users.DTOs;

namespace TabuAI.Application.Features.Users.Queries;

public class GetUserStatisticsQuery : IRequest<List<UserStatisticDto>>
{
    public Guid UserId { get; set; }

    public GetUserStatisticsQuery(Guid userId)
    {
        UserId = userId;
    }
}
