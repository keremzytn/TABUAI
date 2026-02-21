using MediatR;
using TabuAI.Application.Features.Friends.DTOs;

namespace TabuAI.Application.Features.Friends.Queries;

public record GetPendingRequestsQuery(Guid UserId) : IRequest<List<FriendRequestDto>>;
