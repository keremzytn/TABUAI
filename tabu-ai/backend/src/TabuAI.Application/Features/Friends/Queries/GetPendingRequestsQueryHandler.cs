using MediatR;
using Microsoft.EntityFrameworkCore;
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
        return await _unitOfWork.Friendships.AsQueryable()
            .Where(f => f.AddresseeId == request.UserId && f.Status == FriendshipStatus.Pending)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FriendRequestDto
            {
                RequestId = f.Id,
                FromUserId = f.RequesterId,
                FromUsername = f.Requester != null ? f.Requester.Username : "",
                FromDisplayName = f.Requester != null ? f.Requester.DisplayName : null,
                SentAt = f.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
