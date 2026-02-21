using MediatR;

namespace TabuAI.Application.Features.Friends.Commands;

public record RespondFriendRequestCommand(Guid FriendshipId, Guid UserId, bool Accept) : IRequest;
