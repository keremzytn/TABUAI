using MediatR;
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
        var wordPacks = await _unitOfWork.WordPacks.GetAllAsync();
        var words = await _unitOfWork.Words.GetAllAsync();
        var users = await _unitOfWork.Users.GetAllAsync();

        var wordCountByPack = words.Where(w => w.WordPackId.HasValue)
            .GroupBy(w => w.WordPackId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var userDict = users.ToDictionary(u => u.Id, u => u.Username);

        return wordPacks.Select(wp => new AdminWordPackDto
        {
            Id = wp.Id,
            Name = wp.Name,
            Description = wp.Description,
            Language = wp.Language,
            CreatedByUsername = userDict.ContainsKey(wp.CreatedByUserId) ? userDict[wp.CreatedByUserId] : "",
            IsPublic = wp.IsPublic,
            IsApproved = wp.IsApproved,
            PlayCount = wp.PlayCount,
            LikeCount = wp.LikeCount,
            WordCount = wordCountByPack.ContainsKey(wp.Id) ? wordCountByPack[wp.Id] : 0,
            CreatedAt = wp.CreatedAt
        });
    }
}
