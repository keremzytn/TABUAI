using MediatR;
using Microsoft.EntityFrameworkCore;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Users.Queries;

public class GetBadgeGalleryQueryHandler : IRequestHandler<GetBadgeGalleryQuery, BadgeGalleryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBadgeGalleryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BadgeGalleryDto> Handle(GetBadgeGalleryQuery request, CancellationToken cancellationToken)
    {
        var badgesTask = _unitOfWork.Badges.AsQueryable()
            .Where(b => b.IsActive)
            .ToListAsync(cancellationToken);

        var userBadgesTask = _unitOfWork.UserBadges.AsQueryable()
            .Where(ub => ub.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var userTask = _unitOfWork.Users.FindFirstNoTrackingAsync(u => u.Id == request.UserId);

        var sessionStatsTask = _unitOfWork.GameSessions.AsQueryable()
            .Where(s => s.UserId == request.UserId)
            .Select(s => new { s.Score, s.TimeSpent, s.IsCorrectGuess })
            .ToListAsync(cancellationToken);

        var badgeOwnerCountsTask = _unitOfWork.UserBadges.AsQueryable()
            .GroupBy(ub => ub.BadgeId)
            .Select(g => new { BadgeId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.BadgeId, x => x.Count, cancellationToken);

        var totalUsersTask = _unitOfWork.Users.AsQueryable().CountAsync(cancellationToken);

        await Task.WhenAll(badgesTask, userBadgesTask, userTask, sessionStatsTask, badgeOwnerCountsTask, totalUsersTask);

        var badgesList = badgesTask.Result;
        var userBadgeSet = userBadgesTask.Result.ToDictionary(ub => ub.BadgeId);
        var user = userTask.Result;
        var sessions = sessionStatsTask.Result;
        var badgeOwnerCounts = badgeOwnerCountsTask.Result;
        var totalUsers = totalUsersTask.Result;

        int GetCurrentProgress(Badge badge)
        {
            if (user == null) return 0;
            return badge.Type switch
            {
                BadgeType.GamesWon => user.GamesWon,
                BadgeType.PerfectPrompts => sessions.Count(s => s.Score >= 90),
                BadgeType.FastCompleter => sessions.Count(s => s.TimeSpent.TotalSeconds < 30),
                BadgeType.TabuAvoidance => sessions.Count(s => s.IsCorrectGuess),
                BadgeType.HighScore => user.TotalScore,
                BadgeType.Dedication => user.GamesPlayed,
                _ => 0
            };
        }

        string GetRarity(Guid badgeId)
        {
            if (totalUsers <= 0) return "Common";
            var pct = (double)badgeOwnerCounts.GetValueOrDefault(badgeId, 0) / totalUsers * 100;
            return pct switch { < 5 => "Legendary", < 15 => "Epic", < 30 => "Rare", < 60 => "Uncommon", _ => "Common" };
        }

        var earned = new List<BadgeShowcaseDto>();
        var locked = new List<BadgeShowcaseDto>();

        foreach (var badge in badgesList)
        {
            var progress = GetCurrentProgress(badge);
            var progressPct = badge.RequiredValue > 0 ? Math.Min(100, (double)progress / badge.RequiredValue * 100) : 0;
            var isEarned = userBadgeSet.TryGetValue(badge.Id, out var userBadge);

            var dto = new BadgeShowcaseDto
            {
                BadgeId = badge.Id,
                Name = badge.Name,
                Description = badge.Description,
                IconUrl = badge.IconUrl,
                BadgeType = badge.Type.ToString(),
                RequiredValue = badge.RequiredValue,
                CurrentProgress = progress,
                ProgressPercentage = progressPct,
                EarnedAt = userBadge?.EarnedAt,
                IsEarned = isEarned,
                Rarity = GetRarity(badge.Id)
            };

            if (isEarned) earned.Add(dto);
            else locked.Add(dto);
        }

        return new BadgeGalleryDto
        {
            EarnedBadges = earned.OrderByDescending(b => b.EarnedAt).ToList(),
            LockedBadges = locked.OrderByDescending(b => b.ProgressPercentage).ToList(),
            ShowcaseBadgeIds = earned.Take(3).Select(b => b.BadgeId).ToList(),
            TotalBadges = badgesList.Count,
            EarnedCount = earned.Count,
            CompletionPercentage = badgesList.Count > 0 ? (double)earned.Count / badgesList.Count * 100 : 0
        };
    }
}
