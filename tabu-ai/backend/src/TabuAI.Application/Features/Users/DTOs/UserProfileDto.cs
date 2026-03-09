using TabuAI.Domain.Entities;

namespace TabuAI.Application.Features.Users.DTOs;

public class UserProfileDto
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
    public decimal WinRate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<UserBadgeDto> Badges { get; set; } = new();
    public int PromptCoins { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public string? SelectedAvatar { get; set; }
    public string? SelectedCardDesign { get; set; }
}

public class UserBadgeDto
{
    public Guid BadgeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public DateTime EarnedAt { get; set; }
}
