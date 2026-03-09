using MediatR;
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
        var allSessions = await _unitOfWork.GameSessions.GetAllAsync();
        var users = await _unitOfWork.Users.GetAllAsync();
        var words = await _unitOfWork.Words.GetAllAsync();

        var userDict = users.ToDictionary(u => u.Id, u => u.Username);
        var wordDict = words.ToDictionary(w => w.Id, w => w.TargetWord);

        var ordered = allSessions.OrderByDescending(s => s.StartedAt).ToList();
        var totalCount = ordered.Count;

        var items = ordered
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(s => new GameSessionAdminDto
            {
                Id = s.Id,
                Username = userDict.ContainsKey(s.UserId) ? userDict[s.UserId] : "",
                TargetWord = wordDict.ContainsKey(s.WordId) ? wordDict[s.WordId] : "",
                Mode = s.Mode.ToString(),
                Status = s.Status.ToString(),
                IsCorrectGuess = s.IsCorrectGuess,
                Score = s.Score,
                AttemptNumber = s.AttemptNumber,
                StartedAt = s.StartedAt,
                CompletedAt = s.CompletedAt
            }).ToList();

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
