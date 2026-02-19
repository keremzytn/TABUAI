using AutoMapper;
using MediatR;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Users.Queries;

public class GetUserGameHistoryQueryHandler : IRequestHandler<GetUserGameHistoryQuery, List<GameHistoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserGameHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GameHistoryDto>> Handle(GetUserGameHistoryQuery request, CancellationToken cancellationToken)
    {
        // Note: IRepository doesn't support pagination directly on database side efficiently with FindAsync(expr).
        // It returns IEnumerable (in-memory).
        // For production, we need a better repository method: GetPagedAsync(page, size, filter).
        // But for now, we will fetch all user sessions and paginate in memory (acceptable for low volume).
        // OR better: since we filter by UserId, it shouldn't be TOO massive yet.
        
        var sessions = await _unitOfWork.GameSessions.FindAsync(s => s.UserId == request.UserId);
        
        // Manual "Include" Word
        // We only need words for the current page
        var pagedSessions = sessions
            .OrderByDescending(s => s.StartedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var wordIds = pagedSessions.Select(s => s.WordId).Distinct().ToList();
        var words = await _unitOfWork.Words.FindAsync(w => wordIds.Contains(w.Id));

        var historyDtos = new List<GameHistoryDto>();

        foreach (var session in pagedSessions)
        {
            var word = words.FirstOrDefault(w => w.Id == session.WordId);
            historyDtos.Add(new GameHistoryDto
            {
                Id = session.Id,
                TargetWord = word?.TargetWord ?? "Unknown",
                Score = session.Score,
                IsWin = session.Status == TabuAI.Domain.Entities.GameStatus.Completed && session.IsCorrectGuess, // Assuming completed & correct = win
                TimeSpent = session.TimeSpent,
                PlayedAt = session.StartedAt,
                Mode = session.Mode,
                AttemptNumber = session.AttemptNumber
            });
        }

        return historyDtos;
    }
}
