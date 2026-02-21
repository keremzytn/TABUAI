using MediatR;
using Microsoft.AspNetCore.Mvc;
using TabuAI.Application.Features.Friends.Commands;
using TabuAI.Application.Features.Friends.DTOs;
using TabuAI.Application.Features.Friends.Queries;

namespace TabuAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FriendsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FriendsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("request")]
    public async Task<ActionResult<Guid>> SendFriendRequest([FromBody] SendFriendRequestDto dto)
    {
        var command = new SendFriendRequestCommand(dto.RequesterId, dto.AddresseeId);
        var friendshipId = await _mediator.Send(command);
        return Ok(friendshipId);
    }

    [HttpPut("{id}/accept")]
    public async Task<ActionResult> AcceptRequest(Guid id, [FromQuery] Guid userId)
    {
        var command = new RespondFriendRequestCommand(id, userId, true);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPut("{id}/reject")]
    public async Task<ActionResult> RejectRequest(Guid id, [FromQuery] Guid userId)
    {
        var command = new RespondFriendRequestCommand(id, userId, false);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveFriend(Guid id, [FromQuery] Guid userId)
    {
        var command = new RemoveFriendCommand(id, userId);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<List<FriendDto>>> GetFriends(Guid userId)
    {
        var query = new GetFriendsQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{userId}/pending")]
    public async Task<ActionResult<List<FriendRequestDto>>> GetPendingRequests(Guid userId)
    {
        var query = new GetPendingRequestsQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<UserSearchResultDto>>> SearchUsers(
        [FromQuery] string term, [FromQuery] Guid userId)
    {
        var query = new SearchUsersQuery(term, userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

public class SendFriendRequestDto
{
    public Guid RequesterId { get; set; }
    public Guid AddresseeId { get; set; }
}
