using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Responses;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google
{
    /// <summary>
    /// Validates Google Identity Services ID tokens using Google.Apis.Auth.
    /// </summary>
    public class GoogleTokenValidator : IExternalAuthService
    {
        private readonly ILogger<GoogleTokenValidator> _logger;
        private readonly GoogleAuthSettings _settings;

        public GoogleTokenValidator(ILogger<GoogleTokenValidator> logger, IOptions<GoogleAuthSettings> options)
        {
            _logger = logger;
            _settings = options.Value;
        }

        public async Task<ExternalAuthResult> ValidateIdTokenAsync(string idToken)
        {
            if (string.IsNullOrWhiteSpace(idToken))
                return new ExternalAuthResult { Success = false, Error = "ID token is required" };

            try
            {
                var audiences = new List<string>();
                if (!string.IsNullOrWhiteSpace(_settings.ClientId)) audiences.Add(_settings.ClientId);
                if (_settings.AdditionalAudiences is { Count: > 0 }) audiences.AddRange(_settings.AdditionalAudiences);

                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = audiences.Count > 0 ? audiences : null
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

                return new ExternalAuthResult
                {
                    Success = true,
                    Email = payload.Email,
                    ProviderUserId = payload.Subject,
                    Name = string.IsNullOrWhiteSpace(payload.Name) ? payload.Email : payload.Name
                };
            }
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning(ex, "Invalid Google ID token");
                return new ExternalAuthResult { Success = false, Error = "Invalid Google ID token" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google ID token");
                return new ExternalAuthResult { Success = false, Error = "Token validation error" };
            }
        }
    }
}
