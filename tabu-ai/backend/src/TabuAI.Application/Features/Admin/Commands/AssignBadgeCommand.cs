using MediatR;

namespace TabuAI.Application.Features.Admin.Commands;

public record AssignBadgeCommand(Guid BadgeId, Guid UserId) : IRequest<bool>;
