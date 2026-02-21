using MediatR;

namespace TabuAI.Application.Features.Friends.Commands;

public record SendFriendRequestCommand(Guid RequesterId, Guid AddresseeId) : IRequest<Guid>;
