using MediatR;
using TabuAI.Application.Features.Leaderboard.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Leaderboard.Queries;

public class GetLeaderboardQueryHandler : IRequestHandler<GetLeaderboardQuery, LeaderboardResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLeaderboardQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LeaderboardResponse> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        // Get all users with scores
        var allUsers = await _unitOfWork.Users.GetAllAsync();
        var users = allUsers
            .Where(u => u.IsActive && u.GamesPlayed > 0)
            .OrderByDescending(u => u.TotalScore)
            .ToList();

        var totalPlayers = users.Count;

        // Map to DTOs with rank
        var entries = users.Take(request.Top).Select((user, index) => new LeaderboardEntryDto
        {
            Rank = index + 1,
            UserId = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Level = user.Level.ToString(),
            TotalScore = user.TotalScore,
            GamesPlayed = user.GamesPlayed,
            GamesWon = user.GamesWon,
            WinRate = user.WinRate
        }).ToList();

        // Find current user's position
        LeaderboardEntryDto? currentUser = null;
        if (request.CurrentUserId.HasValue)
        {
            var userIndex = users.FindIndex(u => u.Id == request.CurrentUserId.Value);
            if (userIndex >= 0)
            {
                var user = users[userIndex];
                currentUser = new LeaderboardEntryDto
                {
                    Rank = userIndex + 1,
                    UserId = user.Id,
                    Username = user.Username,
                    DisplayName = user.DisplayName,
                    Level = user.Level.ToString(),
                    TotalScore = user.TotalScore,
                    GamesPlayed = user.GamesPlayed,
                    GamesWon = user.GamesWon,
                    WinRate = user.WinRate
                };
            }
        }

        return new LeaderboardResponse
        {
            Entries = entries,
            CurrentUser = currentUser,
            Period = request.Period,
            TotalPlayers = totalPlayers
        };
    }
}
