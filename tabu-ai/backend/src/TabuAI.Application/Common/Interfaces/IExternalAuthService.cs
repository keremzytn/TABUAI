namespace TabuAI.Application.Common.Interfaces;

public interface IExternalAuthService
{
    Task<ExternalAuthUser?> VerifyGoogleTokenAsync(string idToken);
    Task<ExternalAuthUser?> VerifyFacebookTokenAsync(string accessToken);
    Task<ExternalAuthUser?> VerifyMicrosoftTokenAsync(string idToken);
}

public class ExternalAuthUser
{
    public string ExternalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? PictureUrl { get; set; }
}
