using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiquoTrack.StocksipPlatform.IntegrationTests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Get the authorization header
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        
        if (string.IsNullOrEmpty(authHeader))
        {
            // No token provided, authenticate as guest
            var guestClaims = new[] { 
                new Claim(ClaimTypes.NameIdentifier, "guest_user"),
                new Claim(ClaimTypes.Name, "Guest User"),
                new Claim(ClaimTypes.Email, "guest@test.com")
            };
            var guestIdentity = new ClaimsIdentity(guestClaims, "Test");
            var guestPrincipal = new ClaimsPrincipal(guestIdentity);
            var guestTicket = new AuthenticationTicket(guestPrincipal, "Test");
            return Task.FromResult(AuthenticateResult.Success(guestTicket));
        }

        // Parse the token from "Bearer token" format
        const string bearer = "Bearer ";
        if (!authHeader.StartsWith(bearer, StringComparison.OrdinalIgnoreCase))
        {
            // Try treating the whole thing as a token if no Bearer prefix
            authHeader = authHeader.ToString().Trim();
        }
        else
        {
            authHeader = authHeader.Substring(bearer.Length).Trim();
        }

        // Create claims based on token (in test, we just use a mock structure)
        var claims = new[] { 
            new Claim(ClaimTypes.NameIdentifier, "mock_user_id"),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Email, "test@test.com"),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
