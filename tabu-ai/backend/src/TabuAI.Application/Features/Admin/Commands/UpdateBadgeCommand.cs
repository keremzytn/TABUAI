using MediatR;

namespace TabuAI.Application.Features.Admin.Commands;

public record UpdateBadgeCommand(Guid Id, string Name, string Description, string IconUrl, int Type, int RequiredValue, bool IsActive) : IRequest<bool>;
