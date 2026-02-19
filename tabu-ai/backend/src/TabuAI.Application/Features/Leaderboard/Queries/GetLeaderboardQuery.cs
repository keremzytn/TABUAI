using MediatR;
using TabuAI.Application.Features.Leaderboard.DTOs;

namespace TabuAI.Application.Features.Leaderboard.Queries;

public class GetLeaderboardQuery : IRequest<LeaderboardResponse>
{
    public string Period { get; set; } = "AllTime"; // Weekly, Monthly, AllTime
    public Guid? CurrentUserId { get; set; }
    public int Top { get; set; } = 20;
}
