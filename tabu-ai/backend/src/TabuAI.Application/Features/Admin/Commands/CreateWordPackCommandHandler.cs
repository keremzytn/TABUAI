using MediatR;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Commands;

public class CreateWordPackCommandHandler : IRequestHandler<CreateWordPackCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateWordPackCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateWordPackCommand request, CancellationToken cancellationToken)
    {
        var wordPack = new WordPack
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Language = request.Language,
            IsPublic = request.IsPublic,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.WordPacks.AddAsync(wordPack);
        await _unitOfWork.SaveChangesAsync();

        return wordPack.Id;
    }
}
