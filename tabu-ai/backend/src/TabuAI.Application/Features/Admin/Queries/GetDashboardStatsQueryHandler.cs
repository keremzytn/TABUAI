using MediatR;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Queries;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var userList = users.ToList();

        var games = await _unitOfWork.GameSessions.GetAllAsync();
        var gameList = games.ToList();

        var words = await _unitOfWork.Words.GetAllAsync();
        var wordList = words.ToList();

        var today = DateTime.UtcNow.Date;

        // Son 7 gün kayıt istatistikleri
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => today.AddDays(-i))
            .Reverse()
            .Select(date => new DailyRegistrationDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                Count = userList.Count(u => u.CreatedAt.Date == date)
            })
            .ToList();

        // En aktif 5 kullanıcı (skora göre)
        var topUsers = userList
            .OrderByDescending(u => u.TotalScore)
            .Take(5)
            .Select(u => new TopUserDto
            {
                Id = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName ?? u.Username,
                TotalScore = u.TotalScore,
                GamesPlayed = u.GamesPlayed,
                GamesWon = u.GamesWon
            })
            .ToList();

        var badgeCount = await _unitOfWork.Badges.CountAsync();
        var wordPackCount = await _unitOfWork.WordPacks.CountAsync();

        return new DashboardStatsDto
        {
            TotalUsers = userList.Count,
            ActiveUsers = userList.Count(u => u.IsActive),
            InactiveUsers = userList.Count(u => !u.IsActive),
            TotalGames = gameList.Count,
            TodayGames = gameList.Count(g => g.StartedAt.Date == today),
            TotalWords = wordList.Count,
            ActiveWords = wordList.Count(w => w.IsActive),
            InactiveWords = wordList.Count(w => !w.IsActive),
            TotalBadges = badgeCount,
            TotalWordPacks = wordPackCount,
            Last7DaysRegistrations = last7Days,
            TopUsers = topUsers
        };
    }
}
