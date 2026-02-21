using MediatR;
using TabuAI.Application.Features.Friends.DTOs;

namespace TabuAI.Application.Features.Friends.Queries;

public record SearchUsersQuery(string SearchTerm, Guid CurrentUserId) : IRequest<List<UserSearchResultDto>>;
