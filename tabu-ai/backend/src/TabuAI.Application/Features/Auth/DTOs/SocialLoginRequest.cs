namespace TabuAI.Application.Features.Auth.DTOs;

public class SocialLoginRequest
{
    public string Provider { get; set; } = string.Empty; // "GOOGLE" or "FACEBOOK"
    public string IdToken { get; set; } = string.Empty;
}
