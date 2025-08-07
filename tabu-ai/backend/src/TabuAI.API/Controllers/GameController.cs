using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TabuAI.Application.Common.DTOs;
using TabuAI.Application.Features.Game.Commands;

namespace TabuAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GameController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public GameController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Yeni bir oyun başlatır
    /// </summary>
    /// <param name="request">Oyun başlatma isteği</param>
    /// <returns>Başlatılan oyun bilgileri</returns>
    [HttpPost("start")]
    [ProducesResponseType(typeof(GameSessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameSessionDto>> StartGame([FromBody] StartGameRequest request)
    {
        try
        {
            var command = _mapper.Map<StartGameCommand>(request);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Oyun başlatılamadı", error = ex.Message });
        }
    }

    /// <summary>
    /// Prompt gönderir ve AI'den cevap alır
    /// </summary>
    /// <param name="request">Prompt gönderme isteği</param>
    /// <returns>AI'nin tahmini ve değerlendirmesi</returns>
    [HttpPost("submit-prompt")]
    [ProducesResponseType(typeof(GameResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameResultDto>> SubmitPrompt([FromBody] SubmitPromptRequest request)
    {
        try
        {
            var command = _mapper.Map<SubmitPromptCommand>(request);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Prompt işlenemedi", error = ex.Message });
        }
    }

    /// <summary>
    /// Belirli bir oyun oturumunun detaylarını getirir
    /// </summary>
    /// <param name="gameSessionId">Oyun oturumu ID'si</param>
    /// <returns>Oyun oturumu detayları</returns>
    [HttpGet("{gameSessionId}")]
    [ProducesResponseType(typeof(GameSessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<GameSessionDto>> GetGameSession(Guid gameSessionId)
    {
        // Bu endpoint için ayrı bir query oluşturulabilir
        // Şimdilik basit bir implementasyon
        return Task.FromResult<ActionResult<GameSessionDto>>(NotFound(new { message = "Endpoint henüz implement edilmedi" }));
    }

    /// <summary>
    /// Sağlık kontrolü
    /// </summary>
    /// <returns>API durumu</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetHealth()
    {
        return Ok(new 
        { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }
}