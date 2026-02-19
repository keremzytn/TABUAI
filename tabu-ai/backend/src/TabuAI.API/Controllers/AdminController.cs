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

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }

    [HttpPost("users/{userId}/toggle-status")]
    public async Task<ActionResult<bool>> ToggleUserStatus(Guid userId)
    {
        var result = await _mediator.Send(new ToggleUserStatusCommand(userId));
        return Ok(result);
    }

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
}
