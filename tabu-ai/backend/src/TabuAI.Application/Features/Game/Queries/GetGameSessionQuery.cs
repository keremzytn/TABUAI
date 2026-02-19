using MediatR;
using TabuAI.Application.Common.DTOs;

namespace TabuAI.Application.Features.Game.Queries;

public class GetGameSessionQuery : IRequest<GameSessionDto>
{
    public Guid GameSessionId { get; set; }
}
