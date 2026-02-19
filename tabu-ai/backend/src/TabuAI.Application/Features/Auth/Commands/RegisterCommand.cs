using MediatR;
using AutoMapper;
using TabuAI.Application.Features.Auth.DTOs;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Interfaces;
using TabuAI.Application.Common.Interfaces;

namespace TabuAI.Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<AuthResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check uniqueness
        if (await _unitOfWork.Users.ExistsAsync(u => u.Username == request.Username))
        {
            throw new Exception("Username already taken.");
        }

        if (await _unitOfWork.Users.ExistsAsync(u => u.Email == request.Email))
        {
            throw new Exception("Email already registered.");
        }

        // Create User
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            DisplayName = request.DisplayName ?? request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Level = PlayerLevel.Rookie,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user);
        
        // Add Starter Badge? (Example: First Step if they finish a game, maybe not here)
        
        await _unitOfWork.SaveChangesAsync();

        // Generate Token
        var token = _tokenService.GenerateToken(user);
        var userProfile = _mapper.Map<UserProfileDto>(user);

        return new AuthResponse
        {
            Token = token,
            User = userProfile
        };
    }
}
