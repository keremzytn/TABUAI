using MediatR;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Commands;

public class UpdateWordPackCommandHandler : IRequestHandler<UpdateWordPackCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWordPackCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateWordPackCommand request, CancellationToken cancellationToken)
    {
        var wordPack = await _unitOfWork.WordPacks.GetByIdAsync(request.Id);
        if (wordPack == null) return false;

        wordPack.Name = request.Name;
        wordPack.Description = request.Description;
        wordPack.Language = request.Language;
        wordPack.IsPublic = request.IsPublic;
        wordPack.IsApproved = request.IsApproved;
        wordPack.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.WordPacks.UpdateAsync(wordPack);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
