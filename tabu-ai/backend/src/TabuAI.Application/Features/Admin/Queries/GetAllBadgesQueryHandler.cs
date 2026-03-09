using MediatR;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Queries;

public class GetAllBadgesQueryHandler : IRequestHandler<GetAllBadgesQuery, IEnumerable<BadgeDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllBadgesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BadgeDto>> Handle(GetAllBadgesQuery request, CancellationToken cancellationToken)
    {
        var badges = await _unitOfWork.Badges.GetAllAsync();
        var userBadges = await _unitOfWork.UserBadges.GetAllAsync();
        var userBadgeCounts = userBadges.GroupBy(ub => ub.BadgeId).ToDictionary(g => g.Key, g => g.Count());

        return badges.Select(b => new BadgeDto
        {
            Id = b.Id,
            Name = b.Name,
            Description = b.Description,
            IconUrl = b.IconUrl,
            Type = b.Type.ToString(),
            RequiredValue = b.RequiredValue,
            IsActive = b.IsActive,
            CreatedAt = b.CreatedAt,
            UserCount = userBadgeCounts.ContainsKey(b.Id) ? userBadgeCounts[b.Id] : 0
        });
    }
}
