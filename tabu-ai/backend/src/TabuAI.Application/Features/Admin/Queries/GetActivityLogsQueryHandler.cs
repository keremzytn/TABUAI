using MediatR;
using Microsoft.EntityFrameworkCore;
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
        var query = _unitOfWork.ActivityLogs.AsQueryable()
            .OrderByDescending(l => l.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(l => new ActivityLogAdminDto
            {
                Id = l.Id,
                Username = l.User != null ? l.User.Username : "",
                Type = l.Type.ToString(),
                Description = l.Description,
                ScoreEarned = l.ScoreEarned,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync(cancellationToken);

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
