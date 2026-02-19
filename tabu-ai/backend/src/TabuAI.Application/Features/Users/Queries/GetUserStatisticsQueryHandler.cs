using MediatR;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Domain.Interfaces;
using TabuAI.Domain.Entities;

namespace TabuAI.Application.Features.Users.Queries;

public class GetUserStatisticsQueryHandler : IRequestHandler<GetUserStatisticsQuery, List<UserStatisticDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserStatisticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<UserStatisticDto>> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
    {
        var stats = new List<UserStatisticDto>();
        
        // 1. Get base user info for general stats
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) return stats;

        // Add user-level stats
        stats.Add(new UserStatisticDto 
        { 
            MetricName = "Toplam Puan", 
            Value = user.TotalScore, 
            Type = StatisticType.TotalScore,
            FormattedValue = user.TotalScore.ToString("N0")
        });

        stats.Add(new UserStatisticDto 
        { 
            MetricName = "Oyun Sayısı", 
            Value = user.GamesPlayed, 
            Type = StatisticType.TotalScore, // Reusing type approx
            FormattedValue = user.GamesPlayed.ToString("N0")
        });

        stats.Add(new UserStatisticDto 
        { 
            MetricName = "Kazanma Oranı", 
            Value = user.WinRate, 
            Type = StatisticType.SuccessRate,
            FormattedValue = $"%{user.WinRate:F1}"
        });

        // 2. Get detailed statistics from UserStatistic table
        var userStats = await _unitOfWork.UserStatistics.FindAsync(s => s.UserId == request.UserId);
        
        foreach (var stat in userStats)
        {
            string formatted = stat.Type switch
            {
                StatisticType.SuccessRate or StatisticType.CategorySuccessRate or StatisticType.DifficultySuccessRate 
                    => $"%{stat.Value:F1}",
                StatisticType.AverageCompletionTime or StatisticType.AveragePromptLength
                    => $"{stat.Value:F1}",
                _ => stat.Value.ToString("N0")
            };

            stats.Add(new UserStatisticDto
            {
                MetricName = stat.MetricName,
                Value = stat.Value,
                Type = stat.Type,
                FormattedValue = formatted
            });
        }

        return stats;
    }
}
