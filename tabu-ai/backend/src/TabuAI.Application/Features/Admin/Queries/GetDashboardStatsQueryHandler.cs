using MediatR;
using Microsoft.EntityFrameworkCore;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Queries;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private const string DashboardCacheKey = "admin:dashboard_stats";
    private static readonly TimeSpan DashboardCacheTtl = TimeSpan.FromMinutes(5);

    public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync<DashboardStatsDto>(DashboardCacheKey);
        if (cached != null) return cached;

        var today = DateTime.UtcNow.Date;
        var sevenDaysAgo = today.AddDays(-6);

        var userQuery = _unitOfWork.Users.AsQueryable();
        var gameQuery = _unitOfWork.GameSessions.AsQueryable();
        var wordQuery = _unitOfWork.Words.AsQueryable();

        var totalUsersTask = userQuery.CountAsync(cancellationToken);
        var activeUsersTask = userQuery.CountAsync(u => u.IsActive, cancellationToken);
        var totalGamesTask = gameQuery.CountAsync(cancellationToken);
        var todayGamesTask = gameQuery.CountAsync(g => g.StartedAt >= today, cancellationToken);
        var totalWordsTask = wordQuery.CountAsync(cancellationToken);
        var activeWordsTask = wordQuery.CountAsync(w => w.IsActive, cancellationToken);
        var badgeCountTask = _unitOfWork.Badges.CountAsync();
        var wordPackCountTask = _unitOfWork.WordPacks.CountAsync();

        var topUsersTask = userQuery
            .OrderByDescending(u => u.TotalScore)
            .Take(5)
            .Select(u => new TopUserDto
            {
                Id = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName ?? u.Username,
                TotalScore = u.TotalScore,
                GamesPlayed = u.GamesPlayed,
                GamesWon = u.GamesWon
            })
            .ToListAsync(cancellationToken);

        var dailyRegistrationsTask = userQuery
            .Where(u => u.CreatedAt >= sevenDaysAgo)
            .GroupBy(u => u.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        await Task.WhenAll(
            totalUsersTask, activeUsersTask, totalGamesTask, todayGamesTask,
            totalWordsTask, activeWordsTask, badgeCountTask, wordPackCountTask,
            topUsersTask, dailyRegistrationsTask);

        var registrationsByDate = dailyRegistrationsTask.Result.ToDictionary(x => x.Date, x => x.Count);

        var last7Days = Enumerable.Range(0, 7)
            .Select(i => sevenDaysAgo.AddDays(i))
            .Select(date => new DailyRegistrationDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                Count = registrationsByDate.TryGetValue(date, out var c) ? c : 0
            })
            .ToList();

        var totalUsers = totalUsersTask.Result;
        var activeUsers = activeUsersTask.Result;
        var totalWords = totalWordsTask.Result;
        var activeWords = activeWordsTask.Result;

        var result = new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = totalUsers - activeUsers,
            TotalGames = totalGamesTask.Result,
            TodayGames = todayGamesTask.Result,
            TotalWords = totalWords,
            ActiveWords = activeWords,
            InactiveWords = totalWords - activeWords,
            TotalBadges = badgeCountTask.Result,
            TotalWordPacks = wordPackCountTask.Result,
            Last7DaysRegistrations = last7Days,
            TopUsers = topUsersTask.Result
        };

        await _cache.SetAsync(DashboardCacheKey, result, DashboardCacheTtl);

        return result;
    }
}
