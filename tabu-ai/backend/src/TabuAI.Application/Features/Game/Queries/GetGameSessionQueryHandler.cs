using AutoMapper;
using MediatR;
using TabuAI.Application.Common.DTOs;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Game.Queries;

public class GetGameSessionQueryHandler : IRequestHandler<GetGameSessionQuery, GameSessionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetGameSessionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GameSessionDto> Handle(GetGameSessionQuery request, CancellationToken cancellationToken)
    {
        var gameSession = await _unitOfWork.GameSessions.GetByIdAsync(request.GameSessionId);
        if (gameSession == null)
        {
            throw new InvalidOperationException("Oyun oturumu bulunamadı");
        }

        var word = await _unitOfWork.Words.GetByIdAsync(gameSession.WordId);
        if (word != null)
        {
            gameSession.Word = word;
        }

        return _mapper.Map<GameSessionDto>(gameSession);
    }
}
