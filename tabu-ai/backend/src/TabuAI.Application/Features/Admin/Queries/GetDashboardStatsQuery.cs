using MediatR;

namespace TabuAI.Application.Features.Admin.Queries;

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int TotalGames { get; set; }
    public int TodayGames { get; set; }
    public int TotalWords { get; set; }
    public int ActiveWords { get; set; }
    public int InactiveWords { get; set; }
    public int TotalBadges { get; set; }
    public int TotalWordPacks { get; set; }
    public List<DailyRegistrationDto> Last7DaysRegistrations { get; set; } = new();
    public List<TopUserDto> TopUsers { get; set; } = new();
}

public class DailyRegistrationDto
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
}
