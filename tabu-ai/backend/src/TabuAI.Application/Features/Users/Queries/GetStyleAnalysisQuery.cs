using MediatR;
using TabuAI.Application.Features.Users.DTOs;

namespace TabuAI.Application.Features.Users.Queries;

public record GetStyleAnalysisQuery(Guid UserId) : IRequest<StyleAnalysisDto>;
