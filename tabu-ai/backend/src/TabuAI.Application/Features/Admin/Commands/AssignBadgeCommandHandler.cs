using MediatR;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Commands;

public class AssignBadgeCommandHandler : IRequestHandler<AssignBadgeCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignBadgeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AssignBadgeCommand request, CancellationToken cancellationToken)
    {
        var badge = await _unitOfWork.Badges.GetByIdAsync(request.BadgeId);
        if (badge == null) return false;

        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) return false;

        var exists = await _unitOfWork.UserBadges.ExistsAsync(ub => ub.UserId == request.UserId && ub.BadgeId == request.BadgeId);
        if (exists) return false;

        var userBadge = new UserBadge
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            BadgeId = request.BadgeId,
            EarnedAt = DateTime.UtcNow
        };

        await _unitOfWork.UserBadges.AddAsync(userBadge);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
