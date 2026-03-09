using MediatR;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Queries;

public class GetActivityLogsQueryHandler : IRequestHandler<GetActivityLogsQuery, PagedResultDto<ActivityLogAdminDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetActivityLogsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultDto<ActivityLogAdminDto>> Handle(GetActivityLogsQuery request, CancellationToken cancellationToken)
    {
        var allLogs = await _unitOfWork.ActivityLogs.GetAllAsync();
        var users = await _unitOfWork.Users.GetAllAsync();
        var userDict = users.ToDictionary(u => u.Id, u => u.Username);

        var ordered = allLogs.OrderByDescending(l => l.CreatedAt).ToList();
        var totalCount = ordered.Count;

        var items = ordered
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(l => new ActivityLogAdminDto
            {
                Id = l.Id,
                Username = userDict.ContainsKey(l.UserId) ? userDict[l.UserId] : "",
                Type = l.Type.ToString(),
                Description = l.Description,
                ScoreEarned = l.ScoreEarned,
                CreatedAt = l.CreatedAt
            }).ToList();

        return new PagedResultDto<ActivityLogAdminDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            Limit = request.Limit,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.Limit)
        };
    }
}
