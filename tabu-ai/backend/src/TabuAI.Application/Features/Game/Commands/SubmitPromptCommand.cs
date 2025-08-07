using MediatR;
using TabuAI.Application.Common.DTOs;

namespace TabuAI.Application.Features.Game.Commands;

public class SubmitPromptCommand : IRequest<GameResultDto>
{
    public Guid GameSessionId { get; set; }
    public string Prompt { get; set; } = string.Empty;
}