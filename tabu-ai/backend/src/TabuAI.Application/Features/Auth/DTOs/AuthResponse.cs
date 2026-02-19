using TabuAI.Application.Features.Users.DTOs;

namespace TabuAI.Application.Features.Auth.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public UserProfileDto User { get; set; } = null!;
}
