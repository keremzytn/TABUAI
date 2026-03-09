using MediatR;
using AutoMapper;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Domain.Interfaces;

namespace TabuAI.Application.Features.Admin.Queries;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserProfileDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserProfileDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllNoTrackingAsync();
        return _mapper.Map<IEnumerable<UserProfileDto>>(users);
    }
}
