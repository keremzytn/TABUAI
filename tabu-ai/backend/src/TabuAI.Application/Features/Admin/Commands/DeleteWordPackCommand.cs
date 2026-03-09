using MediatR;

namespace TabuAI.Application.Features.Admin.Commands;

public record DeleteWordPackCommand(Guid Id) : IRequest<bool>;
