using MediatR;
using AutoMapper;
using TabuAI.Application.Features.Auth.DTOs;
using TabuAI.Application.Features.Users.DTOs;
using TabuAI.Domain.Interfaces;
using TabuAI.Application.Common.Interfaces;
using TabuAI.Domain.Entities;

namespace TabuAI.Application.Features.Auth.Commands;

public class ExternalLoginCommand : IRequest<AuthResponse>
{
    public string Provider { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
}

public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IExternalAuthService _externalAuthService;
    private readonly IMapper _mapper;

    public ExternalLoginCommandHandler(
        IUnitOfWork unitOfWork, 
        ITokenService tokenService, 
        IExternalAuthService externalAuthService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _externalAuthService = externalAuthService;
        _mapper = mapper;
    }

    public async Task<AuthResponse> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
    {
        ExternalAuthUser? externalUser = null;

        if (request.Provider.ToUpper() == "GOOGLE")
        {
            externalUser = await _externalAuthService.VerifyGoogleTokenAsync(request.IdToken);
        }
        else if (request.Provider.ToUpper() == "FACEBOOK")
        {
            externalUser = await _externalAuthService.VerifyFacebookTokenAsync(request.IdToken);
        }

        if (externalUser == null)
        {
            throw new Exception($"Invalid {request.Provider} token");
        }

        // 1. Find by Social ID
        User? user = null;
        if (request.Provider.ToUpper() == "GOOGLE")
        {
            user = await _unitOfWork.Users.FindFirstAsync(u => u.GoogleId == externalUser.ExternalId);
        }
        else
        {
            user = await _unitOfWork.Users.FindFirstAsync(u => u.FacebookId == externalUser.ExternalId);
        }

        // 2. Find by Email if not found by Social ID
        if (user == null)
        {
            user = await _unitOfWork.Users.FindFirstAsync(u => u.Email == externalUser.Email);
            
            if (user != null)
            {
                // Link account
                if (request.Provider.ToUpper() == "GOOGLE") user.GoogleId = externalUser.ExternalId;
                else user.FacebookId = externalUser.ExternalId;
                
                await _unitOfWork.Users.UpdateAsync(user);
            }
        }

        // 3. Create new user if still not found
        if (user == null)
        {
            // Generate unique username
            var baseUsername = externalUser.Email.Split('@')[0];
            var username = baseUsername;
            int counter = 1;
            
            while (await _unitOfWork.Users.FindFirstAsync(u => u.Username == username) != null)
            {
                username = $"{baseUsername}{counter++}";
            }

            user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = externalUser.Email,
                DisplayName = externalUser.DisplayName ?? username,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                PasswordHash = string.Empty, // No password for social users
                GoogleId = request.Provider.ToUpper() == "GOOGLE" ? externalUser.ExternalId : null,
                FacebookId = request.Provider.ToUpper() == "FACEBOOK" ? externalUser.ExternalId : null
            };

            await _unitOfWork.Users.AddAsync(user);
        }

        user.LastLoginAt = DateTime.UtcNow;
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
