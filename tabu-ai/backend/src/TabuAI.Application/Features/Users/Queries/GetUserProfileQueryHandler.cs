using AutoMapper;
using MediatR;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Domain.Interfaces;
using TabuAI.Domain.Entities;

namespace TabuAI.Application.Features.Users.Queries;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            throw new Exception($"User with ID {request.UserId} not found.");
        }

        var userProfileDto = _mapper.Map<UserProfileDto>(user);
        userProfileDto.PromptCoins = user.PromptCoins;
        userProfileDto.CurrentStreak = user.CurrentStreak;
        userProfileDto.BestStreak = user.BestStreak;
        userProfileDto.SelectedAvatar = user.SelectedAvatar;
        userProfileDto.SelectedCardDesign = user.SelectedCardDesign;

        // Fetch user badges
        var userBadges = await _unitOfWork.UserBadges.FindAsync(ub => ub.UserId == request.UserId);
        var badgeIds = userBadges.Select(ub => ub.BadgeId).ToList();

        if (badgeIds.Any())
        {
            var badges = await _unitOfWork.Badges.FindAsync(b => badgeIds.Contains(b.Id));
            
            foreach (var ub in userBadges)
            {
                var badge = badges.FirstOrDefault(b => b.Id == ub.BadgeId);
                if (badge != null)
                {
                    userProfileDto.Badges.Add(new UserBadgeDto
                    {
                        BadgeId = badge.Id,
                        Name = badge.Name,
                        Description = badge.Description,
                        IconUrl = badge.IconUrl,
                        EarnedAt = ub.EarnedAt
                    });
                }
            }
        }

        return userProfileDto;
    }
}
