using MediatR;
using Microsoft.EntityFrameworkCore;
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
        var userBadgeCounts = await _unitOfWork.UserBadges.AsQueryable()
            .GroupBy(ub => ub.BadgeId)
            .Select(g => new { BadgeId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.BadgeId, x => x.Count, cancellationToken);

        var badges = await _unitOfWork.Badges.GetAllNoTrackingAsync();

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
            UserCount = userBadgeCounts.GetValueOrDefault(b.Id, 0)
        });
    }
}
