using MediatR;

namespace TabuAI.Application.Features.Friends.Commands;

public record RemoveFriendCommand(Guid FriendshipId, Guid UserId) : IRequest;
