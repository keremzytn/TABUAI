using MediatR;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Commands;

public class DeleteBadgeCommandHandler : IRequestHandler<DeleteBadgeCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBadgeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteBadgeCommand request, CancellationToken cancellationToken)
    {
        var badge = await _unitOfWork.Badges.GetByIdAsync(request.Id);
        if (badge == null) return false;

        await _unitOfWork.Badges.DeleteAsync(badge);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
