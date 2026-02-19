using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Common.Services;

public interface IBadgeService
{
    Task CheckAndAwardBadgesAsync(Guid userId);
    Task UpdateUserLevelAsync(Guid userId);
}

public class BadgeService : IBadgeService
{
    private readonly IUnitOfWork _unitOfWork;

    public BadgeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CheckAndAwardBadgesAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return;

        var existingBadges = (await _unitOfWork.UserBadges
            .FindAsync(ub => ub.UserId == userId))
            .Select(ub => ub.BadgeId)
            .ToHashSet();

        var allBadges = await _unitOfWork.Badges.GetAllAsync();

        foreach (var badge in allBadges.Where(b => b.IsActive))
        {
            if (existingBadges.Contains(badge.Id)) continue;

            bool earned = badge.Type switch
            {
                BadgeType.GamesWon => user.GamesWon >= badge.RequiredValue,
                BadgeType.HighScore => user.TotalScore >= badge.RequiredValue,
                BadgeType.LevelUp => (int)user.Level >= badge.RequiredValue,
                _ => false
            };

            if (earned)
            {
                await _unitOfWork.UserBadges.AddAsync(new UserBadge
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    BadgeId = badge.Id,
                    EarnedAt = DateTime.UtcNow
                });
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateUserLevelAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return;

        var newLevel = user.TotalScore switch
        {
            >= 5000 => PlayerLevel.GrandMaster,
            >= 3000 => PlayerLevel.Master,
            >= 1500 => PlayerLevel.Expert,
            >= 700 => PlayerLevel.Skilled,
            >= 300 => PlayerLevel.Apprentice,
            _ => PlayerLevel.Rookie
        };

        if (newLevel != user.Level)
        {
            user.Level = newLevel;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
