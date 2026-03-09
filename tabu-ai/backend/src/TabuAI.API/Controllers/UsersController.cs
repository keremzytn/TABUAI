using MediatR;
using Microsoft.AspNetCore.Mvc;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Application.Features.Users.Queries;

namespace TabuAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId}/profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile(Guid userId)
    {
        var query = new GetUserProfileQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{userId}/statistics")]
    public async Task<ActionResult<List<UserStatisticDto>>> GetStatistics(Guid userId)
    {
        var query = new GetUserStatisticsQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{userId}/history")]
    public async Task<ActionResult<List<GameHistoryDto>>> GetGameHistory(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetUserGameHistoryQuery(userId, page, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{userId}/prompt-analysis")]
    public async Task<ActionResult<PromptAnalysisChartDto>> GetPromptAnalysis(Guid userId, [FromQuery] int days = 30)
    {
        var query = new GetPromptAnalysisChartQuery(userId, days);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{userId}/style-analysis")]
    public async Task<ActionResult<StyleAnalysisDto>> GetStyleAnalysis(Guid userId)
    {
        var query = new GetStyleAnalysisQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{userId}/badge-gallery")]
    public async Task<ActionResult<BadgeGalleryDto>> GetBadgeGallery(Guid userId)
    {
        var query = new GetBadgeGalleryQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
