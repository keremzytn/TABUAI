using MediatR;
using Microsoft.EntityFrameworkCore;
using TabuAI.Application.Features.Friends.DTOs;
using TabuAI.Domain.Enums;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Friends.Queries;

public class GetFriendsQueryHandler : IRequestHandler<GetFriendsQuery, List<FriendDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFriendsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<FriendDto>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        var friendships = await _unitOfWork.Friendships.AsQueryable()
            .Where(f => f.Status == FriendshipStatus.Accepted &&
                        (f.RequesterId == userId || f.AddresseeId == userId))
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .ToListAsync(cancellationToken);

        var friends = friendships.Select(f =>
        {
            var friendUser = f.RequesterId == userId ? f.Addressee : f.Requester;
            return new FriendDto
            {
                FriendshipId = f.Id,
                UserId = friendUser.Id,
                Username = friendUser.Username,
                DisplayName = friendUser.DisplayName,
                Level = friendUser.Level.ToString(),
                TotalScore = friendUser.TotalScore,
                GamesPlayed = friendUser.GamesPlayed,
                GamesWon = friendUser.GamesWon,
                WinRate = friendUser.WinRate,
                FriendSince = f.UpdatedAt ?? f.CreatedAt,
                CreatedAt = friendUser.CreatedAt
            };
        });

        return friends.OrderBy(f => f.Username).ToList();
    }
}
