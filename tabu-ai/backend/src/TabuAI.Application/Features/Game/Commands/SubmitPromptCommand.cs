using MediatR;
using TabuAI.Application.Common.DTOs;

namespace TabuAI.Application.Features.Game.Commands;

public class SubmitPromptCommand : IRequest<GameResultDto>
{
    public string GameSessionId { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string? Persona { get; set; }
}