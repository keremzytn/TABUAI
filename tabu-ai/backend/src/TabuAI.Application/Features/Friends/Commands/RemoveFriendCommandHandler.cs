using MediatR;
using TabuAI.Domain.Enums;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Friends.Commands;

public class RemoveFriendCommandHandler : IRequestHandler<RemoveFriendCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFriendCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
    {
        var friendship = await _unitOfWork.Friendships.GetByIdAsync(request.FriendshipId);
        
        if (friendship == null)
            throw new Exception("Arkadaşlık kaydı bulunamadı.");

        // Either party can remove the friendship
        if (friendship.RequesterId != request.UserId && friendship.AddresseeId != request.UserId)
            throw new Exception("Bu arkadaşlığı kaldırma yetkiniz yok.");

        await _unitOfWork.Friendships.DeleteAsync(friendship);
        await _unitOfWork.SaveChangesAsync();
    }
}
