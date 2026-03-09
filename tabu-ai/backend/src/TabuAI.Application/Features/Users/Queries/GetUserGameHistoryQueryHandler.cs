using MediatR;
using Microsoft.EntityFrameworkCore;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Users.Queries;

public class GetUserGameHistoryQueryHandler : IRequestHandler<GetUserGameHistoryQuery, List<GameHistoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserGameHistoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<GameHistoryDto>> Handle(GetUserGameHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.GameSessions.AsQueryable()
            .Where(s => s.UserId == request.UserId)
            .OrderByDescending(s => s.StartedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new GameHistoryDto
            {
                Id = s.Id,
                TargetWord = s.Word != null ? s.Word.TargetWord : "Unknown",
                Score = s.Score,
                IsWin = s.Status == GameStatus.Completed && s.IsCorrectGuess,
                TimeSpent = s.TimeSpent,
                PlayedAt = s.StartedAt,
                Mode = s.Mode,
                AttemptNumber = s.AttemptNumber
            })
            .ToListAsync(cancellationToken);
    }
}
