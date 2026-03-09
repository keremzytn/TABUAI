using MediatR;

namespace TabuAI.Application.Features.Admin.Commands;

public record CreateWordPackCommand(string Name, string Description, string Language, bool IsPublic) : IRequest<Guid>;
