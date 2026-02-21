using MediatR;
using TabuAI.Application.Features.Friends.DTOs;
using TabuAI.Domain.Enums;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Friends.Queries;

public class GetPendingRequestsQueryHandler : IRequestHandler<GetPendingRequestsQuery, List<FriendRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingRequestsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<FriendRequestDto>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
    {
        var pendingRequests = await _unitOfWork.Friendships.FindAsync(
            f => f.AddresseeId == request.UserId && f.Status == FriendshipStatus.Pending);

        var result = new List<FriendRequestDto>();

        foreach (var friendship in pendingRequests)
        {
            var requester = await _unitOfWork.Users.GetByIdAsync(friendship.RequesterId);
            if (requester != null)
            {
                result.Add(new FriendRequestDto
                {
                    RequestId = friendship.Id,
                    FromUserId = requester.Id,
                    FromUsername = requester.Username,
                    FromDisplayName = requester.DisplayName,
                    SentAt = friendship.CreatedAt
                });
            }
        }

        return result.OrderByDescending(r => r.SentAt).ToList();
    }
}
