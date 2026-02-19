using MediatR;
using TabuAI.Application.Features.Users.DTOs;

namespace TabuAI.Application.Features.Admin.Queries;

public record GetAllUsersQuery : IRequest<IEnumerable<UserProfileDto>>;
