using MediatR;

namespace TabuAI.Application.Features.Admin.Commands;

public record DeleteWordCommand(Guid Id) : IRequest<bool>;
