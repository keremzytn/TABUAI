using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TabuAI.Application.Common.DTOs;
using TabuAI.Application.Features.Admin.Commands;
using TabuAI.Application.Features.Admin.Queries;
using TabuAI.Application.Features.Users.DTOs;

namespace TabuAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ==================== Dashboard ====================

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        var result = await _mediator.Send(new GetDashboardStatsQuery());
        return Ok(result);
    }

    // ==================== Users ====================

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }

    [HttpGet("users/{userId}")]
    public async Task<ActionResult<UserDetailDto>> GetUserDetail(Guid userId)
    {
        var result = await _mediator.Send(new GetUserDetailQuery(userId));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("users/{userId}/toggle-status")]
    public async Task<ActionResult<bool>> ToggleUserStatus(Guid userId)
    {
        var result = await _mediator.Send(new ToggleUserStatusCommand(userId));
        return Ok(result);
    }

    // ==================== Words ====================

    [HttpGet("words")]
    public async Task<ActionResult<IEnumerable<WordDto>>> GetAllWords()
    {
        var result = await _mediator.Send(new GetAllWordsQuery());
        return Ok(result);
    }

    [HttpPost("words")]
    public async Task<ActionResult<Guid>> AddWord(WordDto word)
    {
        var result = await _mediator.Send(new AddWordCommand(word));
        return Ok(result);
    }

    [HttpPut("words")]
    public async Task<ActionResult<bool>> UpdateWord(WordDto word)
    {
        var result = await _mediator.Send(new UpdateWordCommand(word));
        return Ok(result);
    }

    [HttpDelete("words/{id}")]
    public async Task<ActionResult<bool>> DeleteWord(Guid id)
    {
        var result = await _mediator.Send(new DeleteWordCommand(id));
        return Ok(result);
    }

    // ==================== Badges ====================

    [HttpGet("badges")]
    public async Task<ActionResult<IEnumerable<BadgeDto>>> GetAllBadges()
    {
        var result = await _mediator.Send(new GetAllBadgesQuery());
        return Ok(result);
    }

    [HttpPost("badges")]
    public async Task<ActionResult<Guid>> CreateBadge(CreateBadgeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("badges/{id}")]
    public async Task<ActionResult<bool>> UpdateBadge(Guid id, UpdateBadgeCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("badges/{id}")]
    public async Task<ActionResult<bool>> DeleteBadge(Guid id)
    {
        var result = await _mediator.Send(new DeleteBadgeCommand(id));
        return Ok(result);
    }

    [HttpPost("badges/{badgeId}/assign/{userId}")]
    public async Task<ActionResult<bool>> AssignBadge(Guid badgeId, Guid userId)
    {
        var result = await _mediator.Send(new AssignBadgeCommand(badgeId, userId));
        return Ok(result);
    }

    // ==================== Word Packs ====================

    [HttpGet("word-packs")]
    public async Task<ActionResult<IEnumerable<AdminWordPackDto>>> GetAllWordPacks()
    {
        var result = await _mediator.Send(new GetAllWordPacksQuery());
        return Ok(result);
    }

    [HttpPost("word-packs")]
    public async Task<ActionResult<Guid>> CreateWordPack(CreateWordPackCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("word-packs/{id}")]
    public async Task<ActionResult<bool>> UpdateWordPack(Guid id, UpdateWordPackCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("word-packs/{id}")]
    public async Task<ActionResult<bool>> DeleteWordPack(Guid id)
    {
        var result = await _mediator.Send(new DeleteWordPackCommand(id));
        return Ok(result);
    }

    // ==================== Activity ====================

    [HttpGet("game-sessions")]
    public async Task<ActionResult<PagedResultDto<GameSessionAdminDto>>> GetGameSessions([FromQuery] int page = 1, [FromQuery] int limit = 50)
    {
        var result = await _mediator.Send(new GetGameSessionsQuery(page, limit));
        return Ok(result);
    }

    [HttpGet("activity-logs")]
    public async Task<ActionResult<PagedResultDto<ActivityLogAdminDto>>> GetActivityLogs([FromQuery] int page = 1, [FromQuery] int limit = 100)
    {
        var result = await _mediator.Send(new GetActivityLogsQuery(page, limit));
        return Ok(result);
    }
}
