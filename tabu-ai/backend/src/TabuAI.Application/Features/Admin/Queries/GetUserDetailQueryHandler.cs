using MediatR;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Queries;

public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, UserDetailDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserDetailQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDetailDto?> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) return null;

        // Rozetler
        var userBadges = await _unitOfWork.UserBadges.FindAsync(ub => ub.UserId == request.UserId);
        var allBadges = await _unitOfWork.Badges.GetAllAsync();
        var badgeDict = allBadges.ToDictionary(b => b.Id);

        var badges = userBadges.Select(ub => new UserDetailBadgeDto
        {
            BadgeId = ub.BadgeId,
            Name = badgeDict.ContainsKey(ub.BadgeId) ? badgeDict[ub.BadgeId].Name : "",
            Description = badgeDict.ContainsKey(ub.BadgeId) ? badgeDict[ub.BadgeId].Description : "",
            IconUrl = badgeDict.ContainsKey(ub.BadgeId) ? badgeDict[ub.BadgeId].IconUrl : "",
            EarnedAt = ub.EarnedAt
        }).ToList();

        // Son 10 oyun
        var gameSessions = await _unitOfWork.GameSessions.FindAsync(gs => gs.UserId == request.UserId);
        var recentGames = gameSessions
            .OrderByDescending(gs => gs.StartedAt)
            .Take(10)
            .Select(gs => new UserDetailGameSessionDto
            {
                Id = gs.Id,
                TargetWord = gs.Word?.TargetWord ?? "",
                Mode = gs.Mode.ToString(),
                Status = gs.Status.ToString(),
                IsCorrectGuess = gs.IsCorrectGuess,
                Score = gs.Score,
                AttemptNumber = gs.AttemptNumber,
                StartedAt = gs.StartedAt
            }).ToList();

        // Coin işlemleri
        var coinTransactions = await _unitOfWork.CoinTransactions.FindAsync(ct => ct.UserId == request.UserId);
        var recentTransactions = coinTransactions
            .OrderByDescending(ct => ct.CreatedAt)
            .Take(20)
            .Select(ct => new UserDetailCoinTransactionDto
            {
                Id = ct.Id,
                Amount = ct.Amount,
                Type = ct.Type.ToString(),
                Description = ct.Description,
                CreatedAt = ct.CreatedAt
            }).ToList();

        return new UserDetailDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Level = user.Level.ToString(),
            Role = user.Role.ToString(),
            TotalScore = user.TotalScore,
            GamesPlayed = user.GamesPlayed,
            GamesWon = user.GamesWon,
            WinRate = (double)user.WinRate,
            PromptCoins = user.PromptCoins,
            CurrentStreak = user.CurrentStreak,
            BestStreak = user.BestStreak,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            SelectedAvatar = user.SelectedAvatar,
            SelectedCardDesign = user.SelectedCardDesign,
            Badges = badges,
            RecentGames = recentGames,
            CoinTransactions = recentTransactions
        };
    }
}
