using MediatR;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Commands;

public class UpdateBadgeCommandHandler : IRequestHandler<UpdateBadgeCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBadgeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateBadgeCommand request, CancellationToken cancellationToken)
    {
        var badge = await _unitOfWork.Badges.GetByIdAsync(request.Id);
        if (badge == null) return false;

        badge.Name = request.Name;
        badge.Description = request.Description;
        badge.IconUrl = request.IconUrl;
        badge.Type = (BadgeType)request.Type;
        badge.RequiredValue = request.RequiredValue;
        badge.IsActive = request.IsActive;

        await _unitOfWork.Badges.UpdateAsync(badge);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
