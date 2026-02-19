using MediatR;
using AutoMapper;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Commands;

public class UpdateWordCommandHandler : IRequestHandler<UpdateWordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateWordCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateWordCommand request, CancellationToken cancellationToken)
    {
        var existingWord = await _unitOfWork.Words.GetByIdAsync(request.Word.Id);
        if (existingWord == null) return false;

        _mapper.Map(request.Word, existingWord);
        existingWord.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Words.UpdateAsync(existingWord);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
