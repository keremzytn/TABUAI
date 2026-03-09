using MediatR;
using Microsoft.EntityFrameworkCore;
using TabuAI.Application.Features.Leaderboard.DTOs;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Leaderboard.Queries;

public class GetLeaderboardQueryHandler : IRequestHandler<GetLeaderboardQuery, LeaderboardResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private static readonly TimeSpan LeaderboardCacheTtl = TimeSpan.FromMinutes(2);

    public GetLeaderboardQueryHandler(IUnitOfWork unitOfWork, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<LeaderboardResponse> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"leaderboard:{request.Period}:{request.Top}";
        var cached = await _cache.GetAsync<LeaderboardResponse>(cacheKey);
        if (cached != null)
        {
            if (request.CurrentUserId.HasValue)
                cached.CurrentUser = cached.Entries.FirstOrDefault(e => e.UserId == request.CurrentUserId.Value);
            return cached;
        }

        var baseQuery = _unitOfWork.Users.AsQueryable()
            .Where(u => u.IsActive && u.GamesPlayed > 0)
            .OrderByDescending(u => u.TotalScore);

        var totalPlayers = await baseQuery.CountAsync(cancellationToken);

        var topUsers = await baseQuery
            .Take(request.Top)
            .Select(u => new LeaderboardEntryDto
            {
                UserId = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName,
                Level = u.Level.ToString(),
                TotalScore = u.TotalScore,
                GamesPlayed = u.GamesPlayed,
                GamesWon = u.GamesWon,
                WinRate = u.WinRate
            })
            .ToListAsync(cancellationToken);

        for (int i = 0; i < topUsers.Count; i++)
            topUsers[i].Rank = i + 1;

        LeaderboardEntryDto? currentUser = null;
        if (request.CurrentUserId.HasValue)
        {
            var existingEntry = topUsers.FirstOrDefault(u => u.UserId == request.CurrentUserId.Value);
            if (existingEntry != null)
            {
                currentUser = existingEntry;
            }
            else
            {
                var userScore = await _unitOfWork.Users.AsQueryable()
                    .Where(x => x.Id == request.CurrentUserId.Value)
                    .Select(x => x.TotalScore)
                    .FirstOrDefaultAsync(cancellationToken);

                var rank = await baseQuery
                    .Where(u => u.TotalScore > userScore)
                    .CountAsync(cancellationToken) + 1;

                var cu = await _unitOfWork.Users.AsQueryable()
                    .Where(u => u.Id == request.CurrentUserId.Value && u.IsActive)
                    .Select(u => new LeaderboardEntryDto
                    {
                        Rank = rank,
                        UserId = u.Id,
                        Username = u.Username,
                        DisplayName = u.DisplayName,
                        Level = u.Level.ToString(),
                        TotalScore = u.TotalScore,
                        GamesPlayed = u.GamesPlayed,
                        GamesWon = u.GamesWon,
                        WinRate = u.WinRate
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                currentUser = cu;
            }
        }

        var response = new LeaderboardResponse
        {
            Entries = topUsers,
            CurrentUser = currentUser,
            Period = request.Period,
            TotalPlayers = totalPlayers
        };

        await _cache.SetAsync(cacheKey, response, LeaderboardCacheTtl);

        return response;
    }
}
