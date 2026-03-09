using MediatR;
using TabuAI.Application.Features.Users.DTOs;

namespace TabuAI.Application.Features.Users.Queries;

public record GetPromptAnalysisChartQuery(Guid UserId, int Days = 30) : IRequest<PromptAnalysisChartDto>;
