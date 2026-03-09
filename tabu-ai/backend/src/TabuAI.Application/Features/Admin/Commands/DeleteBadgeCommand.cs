using MediatR;

namespace TabuAI.Application.Features.Admin.Commands;

public record DeleteBadgeCommand(Guid Id) : IRequest<bool>;
