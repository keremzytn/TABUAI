using MediatR;
using AutoMapper;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Commands;

public class AddWordCommandHandler : IRequestHandler<AddWordCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddWordCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(AddWordCommand request, CancellationToken cancellationToken)
    {
        var word = _mapper.Map<Word>(request.Word);
        word.Id = Guid.NewGuid();
        word.CreatedAt = DateTime.UtcNow;
        word.IsActive = true;

        await _unitOfWork.Words.AddAsync(word);
        await _unitOfWork.SaveChangesAsync();

        return word.Id;
    }
}
