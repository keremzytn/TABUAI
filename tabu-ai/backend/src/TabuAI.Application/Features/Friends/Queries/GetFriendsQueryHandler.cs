using MediatR;
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
        // Get friendships where user is requester or addressee AND status is Accepted
        var sentAccepted = await _unitOfWork.Friendships.FindAsync(
            f => f.RequesterId == request.UserId && f.Status == FriendshipStatus.Accepted);
        
        var receivedAccepted = await _unitOfWork.Friendships.FindAsync(
            f => f.AddresseeId == request.UserId && f.Status == FriendshipStatus.Accepted);

        var friends = new List<FriendDto>();

        foreach (var f in sentAccepted)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(f.AddresseeId);
            if (user != null)
            {
                friends.Add(new FriendDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    DisplayName = user.DisplayName,
                    Level = user.Level.ToString(),
                    TotalScore = user.TotalScore,
                    FriendSince = f.UpdatedAt ?? f.CreatedAt
                });
            }
        }

        foreach (var f in receivedAccepted)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(f.RequesterId);
            if (user != null)
            {
                friends.Add(new FriendDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    DisplayName = user.DisplayName,
                    Level = user.Level.ToString(),
                    TotalScore = user.TotalScore,
                    FriendSince = f.UpdatedAt ?? f.CreatedAt
                });
            }
        }

        return friends.OrderBy(f => f.Username).ToList();
    }
}
