using MediatR;
using AutoMapper;
using TabuAI.Application.Common.DTOs;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Queries;

public class GetAllWordsQueryHandler : IRequestHandler<GetAllWordsQuery, IEnumerable<WordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllWordsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WordDto>> Handle(GetAllWordsQuery request, CancellationToken cancellationToken)
    {
        var words = await _unitOfWork.Words.GetAllNoTrackingAsync();
        return _mapper.Map<IEnumerable<WordDto>>(words);
    }
}
