using MediatR;

namespace TabuAI.Application.Features.Admin.Commands;

public record UpdateWordPackCommand(Guid Id, string Name, string Description, string Language, bool IsPublic, bool IsApproved) : IRequest<bool>;
