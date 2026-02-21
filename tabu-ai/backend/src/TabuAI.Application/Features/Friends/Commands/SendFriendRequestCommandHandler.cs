using MediatR;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Enums;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Friends.Commands;

public class SendFriendRequestCommandHandler : IRequestHandler<SendFriendRequestCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public SendFriendRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
    {
        if (request.RequesterId == request.AddresseeId)
            throw new Exception("Kendinize arkadaşlık isteği gönderemezsiniz.");

        // Check if addressee exists
        var addressee = await _unitOfWork.Users.GetByIdAsync(request.AddresseeId);
        if (addressee == null)
            throw new Exception("Kullanıcı bulunamadı.");

        // Check if a friendship already exists in either direction
        var existingForward = await _unitOfWork.Friendships.FindFirstAsync(
            f => f.RequesterId == request.RequesterId && f.AddresseeId == request.AddresseeId);
        
        var existingReverse = await _unitOfWork.Friendships.FindFirstAsync(
            f => f.RequesterId == request.AddresseeId && f.AddresseeId == request.RequesterId);

        if (existingForward != null)
        {
            if (existingForward.Status == FriendshipStatus.Accepted)
                throw new Exception("Bu kullanıcı zaten arkadaşınız.");
            if (existingForward.Status == FriendshipStatus.Pending)
                throw new Exception("Bu kullanıcıya zaten istek gönderdiniz.");
            // If rejected, allow re-sending by updating
            existingForward.Status = FriendshipStatus.Pending;
            existingForward.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Friendships.UpdateAsync(existingForward);
            await _unitOfWork.SaveChangesAsync();
            return existingForward.Id;
        }

        if (existingReverse != null)
        {
            if (existingReverse.Status == FriendshipStatus.Accepted)
                throw new Exception("Bu kullanıcı zaten arkadaşınız.");
            if (existingReverse.Status == FriendshipStatus.Pending)
                throw new Exception("Bu kullanıcı size zaten istek göndermiş.");
        }

        var friendship = new Friendship
        {
            Id = Guid.NewGuid(),
            RequesterId = request.RequesterId,
            AddresseeId = request.AddresseeId,
            Status = FriendshipStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Friendships.AddAsync(friendship);
        await _unitOfWork.SaveChangesAsync();

        return friendship.Id;
    }
}
