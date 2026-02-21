using MediatR;
using TabuAI.Application.Features.Friends.DTOs;
using TabuAI.Domain.Enums;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Friends.Queries;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<UserSearchResultDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<UserSearchResultDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm) || request.SearchTerm.Length < 2)
            return new List<UserSearchResultDto>();

        var searchLower = request.SearchTerm.ToLower();

        var users = await _unitOfWork.Users.FindAsync(
            u => u.Id != request.CurrentUserId 
                 && u.Role != UserRole.Admin
                 && (u.Username.ToLower().Contains(searchLower) 
                     || (u.DisplayName != null && u.DisplayName.ToLower().Contains(searchLower))));

        var results = new List<UserSearchResultDto>();

        foreach (var user in users.Take(20))
        {
            // Check friendship status
            string? friendshipStatus = null;

            var forward = await _unitOfWork.Friendships.FindFirstAsync(
                f => f.RequesterId == request.CurrentUserId && f.AddresseeId == user.Id
                     && f.Status != FriendshipStatus.Rejected);

            var reverse = await _unitOfWork.Friendships.FindFirstAsync(
                f => f.RequesterId == user.Id && f.AddresseeId == request.CurrentUserId
                     && f.Status != FriendshipStatus.Rejected);

            if (forward != null)
                friendshipStatus = forward.Status.ToString();
            else if (reverse != null)
                friendshipStatus = reverse.Status.ToString();

            results.Add(new UserSearchResultDto
            {
                UserId = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Level = user.Level.ToString(),
                FriendshipStatus = friendshipStatus
            });
        }

        return results;
    }
}
