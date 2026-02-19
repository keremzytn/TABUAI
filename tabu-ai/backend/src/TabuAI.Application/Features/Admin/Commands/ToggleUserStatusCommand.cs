using MediatR;

namespace TabuAI.Application.Features.Admin.Commands;

public record ToggleUserStatusCommand(Guid UserId) : IRequest<bool>;
