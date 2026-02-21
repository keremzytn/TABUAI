using MediatR;
using TabuAI.Application.Features.Friends.DTOs;

namespace TabuAI.Application.Features.Friends.Queries;

public record GetFriendsQuery(Guid UserId) : IRequest<List<FriendDto>>;
