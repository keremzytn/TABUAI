using MediatR;

namespace TabuAI.Application.Features.Admin.Commands;

public record CreateBadgeCommand(string Name, string Description, string IconUrl, int Type, int RequiredValue) : IRequest<Guid>;
