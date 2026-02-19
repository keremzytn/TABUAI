using MediatR;
using Microsoft.AspNetCore.Mvc;
using TabuAI.Application.Features.Leaderboard.DTOs;
using TabuAI.Application.Features.Leaderboard.Queries;

namespace TabuAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LeaderboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaderboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Liderlik tablosunu getirir
    /// </summary>
    /// <param name="period">Dönem: Weekly, Monthly, AllTime</param>
    /// <param name="userId">Mevcut kullanıcı ID (opsiyonel)</param>
    /// <param name="top">Gösterilecek oyuncu sayısı</param>
    /// <returns>Liderlik tablosu verileri</returns>
    [HttpGet]
    [ProducesResponseType(typeof(LeaderboardResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LeaderboardResponse>> GetLeaderboard(
        [FromQuery] string period = "AllTime",
        [FromQuery] Guid? userId = null,
        [FromQuery] int top = 20)
    {
        try
        {
            var query = new GetLeaderboardQuery
            {
                Period = period,
                CurrentUserId = userId,
                Top = top
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Liderlik tablosu yüklenemedi", error = ex.Message });
        }
    }
}
