using MediatR;
using TabuAI.Application.Common.DTOs;

namespace TabuAI.Application.Features.Admin.Queries;

public record GetAllWordsQuery : IRequest<IEnumerable<WordDto>>;
