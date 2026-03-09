using MediatR;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Commands;

public class DeleteWordPackCommandHandler : IRequestHandler<DeleteWordPackCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWordPackCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteWordPackCommand request, CancellationToken cancellationToken)
    {
        var wordPack = await _unitOfWork.WordPacks.GetByIdAsync(request.Id);
        if (wordPack == null) return false;

        await _unitOfWork.WordPacks.DeleteAsync(wordPack);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
