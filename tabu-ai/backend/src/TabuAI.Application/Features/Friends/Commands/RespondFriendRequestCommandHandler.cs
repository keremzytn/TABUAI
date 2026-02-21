using MediatR;
using TabuAI.Domain.Enums;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Friends.Commands;

public class RespondFriendRequestCommandHandler : IRequestHandler<RespondFriendRequestCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RespondFriendRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RespondFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var friendship = await _unitOfWork.Friendships.GetByIdAsync(request.FriendshipId);
        
        if (friendship == null)
            throw new Exception("Arkadaşlık isteği bulunamadı.");

        if (friendship.AddresseeId != request.UserId)
            throw new Exception("Bu isteğe yanıt verme yetkiniz yok.");

        if (friendship.Status != FriendshipStatus.Pending)
            throw new Exception("Bu istek zaten yanıtlanmış.");

        friendship.Status = request.Accept ? FriendshipStatus.Accepted : FriendshipStatus.Rejected;
        friendship.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Friendships.UpdateAsync(friendship);
        await _unitOfWork.SaveChangesAsync();
    }
}
