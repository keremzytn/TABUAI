using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using TabuAI.Application.Common.Interfaces;

namespace TabuAI.Infrastructure.Services;

public class ExternalAuthService : IExternalAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExternalAuthService> _logger;
    private readonly HttpClient _httpClient;

    public ExternalAuthService(IConfiguration configuration, ILogger<ExternalAuthService> logger, HttpClient httpClient)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<ExternalAuthUser?> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            var googleClientId = _configuration["Authentication:Google:ClientId"];
            
            // Note: In development, you might want to skip audience validation if you don't have the ClientId yet
            // But for production, it's mandatory.
            var settings = new GoogleJsonWebSignature.ValidationSettings();
            if (!string.IsNullOrEmpty(googleClientId))
            {
                settings.Audience = new[] { googleClientId };
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new ExternalAuthUser
            {
                ExternalId = payload.Subject,
                Email = payload.Email,
                DisplayName = payload.Name,
                PictureUrl = payload.Picture
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google token verification failed");
            return null;
        }
    }

    public async Task<ExternalAuthUser?> VerifyFacebookTokenAsync(string accessToken)
    {
        try
        {
            // Facebook Graph API call
            var url = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={accessToken}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Facebook API returned error: {StatusCode}", response.StatusCode);
                return null;
            }

            var fbUser = await response.Content.ReadFromJsonAsync<FacebookUserResponse>();
            
            if (fbUser == null) return null;

            return new ExternalAuthUser
            {
                ExternalId = fbUser.Id,
                Email = fbUser.Email ?? $"{fbUser.Id}@facebook.com", // Facebook sometimes doesn't return email
                DisplayName = fbUser.Name,
                PictureUrl = fbUser.Picture?.Data?.Url
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Facebook token verification failed");
            return null;
        }
    }

    private class FacebookUserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public FacebookPicture? Picture { get; set; }
    }

    private class FacebookPicture
    {
        public FacebookPictureData? Data { get; set; }
    }

    private class FacebookPictureData
    {
        public string? Url { get; set; }
    }
}
