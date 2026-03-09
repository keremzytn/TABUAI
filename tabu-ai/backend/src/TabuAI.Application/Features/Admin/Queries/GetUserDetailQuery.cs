using MediatR;

namespace TabuAI.Application.Features.Admin.Queries;

public record GetUserDetailQuery(Guid UserId) : IRequest<UserDetailDto?>;

public class UserDetailDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public double WinRate { get; set; }
    public int PromptCoins { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? SelectedAvatar { get; set; }
    public string? SelectedCardDesign { get; set; }
    public List<UserDetailBadgeDto> Badges { get; set; } = new();
    public List<UserDetailGameSessionDto> RecentGames { get; set; } = new();
    public List<UserDetailCoinTransactionDto> CoinTransactions { get; set; } = new();
}

public class UserDetailBadgeDto
{
    public Guid BadgeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; }
}

public class UserDetailGameSessionDto
{
    public Guid Id { get; set; }
    public string TargetWord { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsCorrectGuess { get; set; }
    public int Score { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime StartedAt { get; set; }
}

public class UserDetailCoinTransactionDto
{
    public Guid Id { get; set; }
    public int Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
