using MediatR;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Commands;

public class CreateBadgeCommandHandler : IRequestHandler<CreateBadgeCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBadgeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateBadgeCommand request, CancellationToken cancellationToken)
    {
        var badge = new Badge
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            IconUrl = request.IconUrl,
            Type = (BadgeType)request.Type,
            RequiredValue = request.RequiredValue,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Badges.AddAsync(badge);
        await _unitOfWork.SaveChangesAsync();

        return badge.Id;
    }
}
