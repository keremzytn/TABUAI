using MediatR;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Users.Queries;

public class GetPromptAnalysisChartQueryHandler : IRequestHandler<GetPromptAnalysisChartQuery, PromptAnalysisChartDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPromptAnalysisChartQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PromptAnalysisChartDto> Handle(GetPromptAnalysisChartQuery request, CancellationToken cancellationToken)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-request.Days);

        var sessions = await _unitOfWork.GameSessions.FindAsync(s =>
            s.UserId == request.UserId && s.StartedAt >= cutoffDate);

        var sessionList = sessions.ToList();
        var sessionIds = sessionList.Select(s => s.Id).ToList();

        var attempts = await _unitOfWork.GameAttempts.FindAsync(a => sessionIds.Contains(a.GameSessionId));
        var attemptsList = attempts.ToList();

        var grouped = sessionList
            .GroupBy(s => s.StartedAt.Date)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var daySessionIds = g.Select(s => s.Id).ToHashSet();
                var dayAttempts = attemptsList
                    .Where(a => daySessionIds.Contains(a.GameSessionId))
                    .ToList();

                var allWords = dayAttempts
                    .SelectMany(a => a.UserPrompt.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    .Select(w => w.ToLowerInvariant().Trim())
                    .Where(w => w.Length > 1)
                    .ToList();

                return new PromptAnalysisDataPoint
                {
                    Date = g.Key,
                    AveragePromptLength = dayAttempts.Count > 0 ? dayAttempts.Average(a => a.UserPrompt.Length) : 0,
                    SuccessRate = g.Count() > 0 ? (double)g.Count(s => s.IsCorrectGuess) / g.Count() * 100 : 0,
                    UniqueWordsUsed = allWords.Distinct().Count(),
                    GamesPlayed = g.Count(),
                    AverageScore = g.Average(s => s.Score)
                };
            })
            .ToList();

        return new PromptAnalysisChartDto { DataPoints = grouped };
    }
}
