using MediatR;
using Microsoft.EntityFrameworkCore;
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
        var user = await _unitOfWork.Users.FindFirstNoTrackingAsync(u => u.Id == request.UserId);
        if (user == null) return null;

        var badgesTask = _unitOfWork.UserBadges.AsQueryable()
            .Where(ub => ub.UserId == request.UserId)
            .Select(ub => new UserDetailBadgeDto
            {
                BadgeId = ub.BadgeId,
                Name = ub.Badge != null ? ub.Badge.Name : "",
                Description = ub.Badge != null ? ub.Badge.Description : "",
                IconUrl = ub.Badge != null ? ub.Badge.IconUrl : "",
                EarnedAt = ub.EarnedAt
            })
            .ToListAsync(cancellationToken);

        var recentGamesTask = _unitOfWork.GameSessions.AsQueryable()
            .Where(gs => gs.UserId == request.UserId)
            .OrderByDescending(gs => gs.StartedAt)
            .Take(10)
            .Select(gs => new UserDetailGameSessionDto
            {
                Id = gs.Id,
                TargetWord = gs.Word != null ? gs.Word.TargetWord : "",
                Mode = gs.Mode.ToString(),
                Status = gs.Status.ToString(),
                IsCorrectGuess = gs.IsCorrectGuess,
                Score = gs.Score,
                AttemptNumber = gs.AttemptNumber,
                StartedAt = gs.StartedAt
            })
            .ToListAsync(cancellationToken);

        var recentTransactionsTask = _unitOfWork.CoinTransactions.AsQueryable()
            .Where(ct => ct.UserId == request.UserId)
            .OrderByDescending(ct => ct.CreatedAt)
            .Take(20)
            .Select(ct => new UserDetailCoinTransactionDto
            {
                Id = ct.Id,
                Amount = ct.Amount,
                Type = ct.Type.ToString(),
                Description = ct.Description,
                CreatedAt = ct.CreatedAt
            })
            .ToListAsync(cancellationToken);

        await Task.WhenAll(badgesTask, recentGamesTask, recentTransactionsTask);

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
            Badges = badgesTask.Result,
            RecentGames = recentGamesTask.Result,
            CoinTransactions = recentTransactionsTask.Result
        };
    }
}
