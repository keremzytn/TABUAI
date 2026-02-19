using MediatR;
using TabuAI.Application.Features.Users.DTOs;

namespace TabuAI.Application.Features.Users.Queries;

public class GetUserProfileQuery : IRequest<UserProfileDto>
{
    public Guid UserId { get; set; }

    public GetUserProfileQuery(Guid userId)
    {
        UserId = userId;
    }
}
