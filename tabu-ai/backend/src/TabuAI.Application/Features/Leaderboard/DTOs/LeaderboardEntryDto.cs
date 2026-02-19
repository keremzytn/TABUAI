namespace TabuAI.Application.Features.Leaderboard.DTOs;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Level { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public decimal WinRate { get; set; }
}

public class LeaderboardResponse
{
    public List<LeaderboardEntryDto> Entries { get; set; } = new();
    public LeaderboardEntryDto? CurrentUser { get; set; }
    public string Period { get; set; } = "AllTime";
    public int TotalPlayers { get; set; }
}
