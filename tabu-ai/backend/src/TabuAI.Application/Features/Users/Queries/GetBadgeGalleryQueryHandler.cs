using MediatR;
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
        var allBadges = await _unitOfWork.Badges.GetAllAsync();
        var userBadges = await _unitOfWork.UserBadges.FindAsync(ub => ub.UserId == request.UserId);
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        var sessions = await _unitOfWork.GameSessions.FindAsync(s => s.UserId == request.UserId);

        var badgesList = allBadges.Where(b => b.IsActive).ToList();
        var userBadgesList = userBadges.ToList();
        var sessionsList = sessions.ToList();

        var allUserBadges = await _unitOfWork.UserBadges.GetAllAsync();
        var totalUsers = (await _unitOfWork.Users.GetAllAsync()).Count();
        var badgeOwnerCounts = allUserBadges.GroupBy(ub => ub.BadgeId).ToDictionary(g => g.Key, g => g.Count());

        int GetCurrentProgress(Badge badge)
        {
            if (user == null) return 0;
            return badge.Type switch
            {
                BadgeType.GamesWon => user.GamesWon,
                BadgeType.PerfectPrompts => sessionsList.Count(s => s.Score >= 90),
                BadgeType.FastCompleter => sessionsList.Count(s => s.TimeSpent.TotalSeconds < 30),
                BadgeType.TabuAvoidance => sessionsList.Count(s => s.IsCorrectGuess),
                BadgeType.HighScore => user.TotalScore,
                BadgeType.Dedication => user.GamesPlayed,
                _ => 0
            };
        }

        string GetRarity(Guid badgeId)
        {
            if (totalUsers <= 0) return "Common";
            var ownerCount = badgeOwnerCounts.GetValueOrDefault(badgeId, 0);
            var pct = (double)ownerCount / totalUsers * 100;
            return pct switch
            {
                < 5 => "Legendary",
                < 15 => "Epic",
                < 30 => "Rare",
                < 60 => "Uncommon",
                _ => "Common"
            };
        }

        var earned = new List<BadgeShowcaseDto>();
        var locked = new List<BadgeShowcaseDto>();

        foreach (var badge in badgesList)
        {
            var userBadge = userBadgesList.FirstOrDefault(ub => ub.BadgeId == badge.Id);
            var progress = GetCurrentProgress(badge);
            var progressPct = badge.RequiredValue > 0 ? Math.Min(100, (double)progress / badge.RequiredValue * 100) : 0;

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
                IsEarned = userBadge != null,
                Rarity = GetRarity(badge.Id)
            };

            if (userBadge != null)
                earned.Add(dto);
            else
                locked.Add(dto);
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
