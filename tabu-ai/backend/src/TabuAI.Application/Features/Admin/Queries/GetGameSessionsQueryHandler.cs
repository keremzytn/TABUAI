using MediatR;
using Microsoft.EntityFrameworkCore;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Queries;

public class GetGameSessionsQueryHandler : IRequestHandler<GetGameSessionsQuery, PagedResultDto<GameSessionAdminDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetGameSessionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultDto<GameSessionAdminDto>> Handle(GetGameSessionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.GameSessions.AsQueryable()
            .OrderByDescending(s => s.StartedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(s => new GameSessionAdminDto
            {
                Id = s.Id,
                Username = s.User != null ? s.User.Username : "",
                TargetWord = s.Word != null ? s.Word.TargetWord : "",
                Mode = s.Mode.ToString(),
                Status = s.Status.ToString(),
                IsCorrectGuess = s.IsCorrectGuess,
                Score = s.Score,
                AttemptNumber = s.AttemptNumber,
                StartedAt = s.StartedAt,
                CompletedAt = s.CompletedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<GameSessionAdminDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            Limit = request.Limit,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.Limit)
        };
    }
}
