using MediatR;
using TabuAI.Application.Features.Users.DTOs;

namespace TabuAI.Application.Features.Users.Queries;

public record GetBadgeGalleryQuery(Guid UserId) : IRequest<BadgeGalleryDto>;
