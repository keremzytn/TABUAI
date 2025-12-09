using MediatR;
using TabuAI.Application.Common.DTOs;

namespace TabuAI.Application.Features.Game.Commands;

public class StartGameCommand : IRequest<GameSessionDto>
{
    public string UserId { get; set; } = string.Empty;
    public string GameMode { get; set; } = "Solo";
    public string? Category { get; set; }
    public int? Difficulty { get; set; }
}