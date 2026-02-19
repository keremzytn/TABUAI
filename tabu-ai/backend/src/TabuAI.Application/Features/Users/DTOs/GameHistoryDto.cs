using TabuAI.Domain.Entities;

namespace TabuAI.Application.Features.Users.DTOs;

public class GameHistoryDto
{
    public Guid Id { get; set; }
    public string TargetWord { get; set; } = string.Empty;
    public int Score { get; set; }
    public bool IsWin { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public DateTime PlayedAt { get; set; }
    public GameMode Mode { get; set; }
    public int AttemptNumber { get; set; }
}
