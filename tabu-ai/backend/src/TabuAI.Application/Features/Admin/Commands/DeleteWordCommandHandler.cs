using MediatR;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Commands;

public class DeleteWordCommandHandler : IRequestHandler<DeleteWordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWordCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteWordCommand request, CancellationToken cancellationToken)
    {
        var word = await _unitOfWork.Words.GetByIdAsync(request.Id);
        if (word == null) return false;

        await _unitOfWork.Words.DeleteAsync(word);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
