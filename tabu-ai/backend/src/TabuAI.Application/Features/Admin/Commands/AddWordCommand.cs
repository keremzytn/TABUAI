using MediatR;
using TabuAI.Application.Common.DTOs;

namespace TabuAI.Application.Features.Admin.Commands;

public record AddWordCommand(WordDto Word) : IRequest<Guid>;
