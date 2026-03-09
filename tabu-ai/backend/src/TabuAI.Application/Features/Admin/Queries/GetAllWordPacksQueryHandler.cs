using MediatR;
using Microsoft.EntityFrameworkCore;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Queries;

public class GetAllWordPacksQueryHandler : IRequestHandler<GetAllWordPacksQuery, IEnumerable<AdminWordPackDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllWordPacksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AdminWordPackDto>> Handle(GetAllWordPacksQuery request, CancellationToken cancellationToken)
    {
        var wordCountByPack = await _unitOfWork.Words.AsQueryable()
            .Where(w => w.WordPackId.HasValue)
            .GroupBy(w => w.WordPackId!.Value)
            .Select(g => new { PackId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PackId, x => x.Count, cancellationToken);

        return await _unitOfWork.WordPacks.AsQueryable()
            .Select(wp => new AdminWordPackDto
            {
                Id = wp.Id,
                Name = wp.Name,
                Description = wp.Description,
                Language = wp.Language,
                CreatedByUsername = wp.CreatedByUser != null ? wp.CreatedByUser.Username : "",
                IsPublic = wp.IsPublic,
                IsApproved = wp.IsApproved,
                PlayCount = wp.PlayCount,
                LikeCount = wp.LikeCount,
                WordCount = 0,
                CreatedAt = wp.CreatedAt
            })
            .ToListAsync(cancellationToken)
            .ContinueWith(t =>
            {
                foreach (var wp in t.Result)
                    wp.WordCount = wordCountByPack.GetValueOrDefault(wp.Id, 0);
                return (IEnumerable<AdminWordPackDto>)t.Result;
            }, cancellationToken);
    }
}
