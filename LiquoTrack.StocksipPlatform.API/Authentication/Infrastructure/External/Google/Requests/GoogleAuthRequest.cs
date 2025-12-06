namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Requests
{
    /// <summary>
    /// Request payload for Google Sign-In containing the ID token issued by Google Identity Services.
    /// </summary>
    public record GoogleAuthRequest(string IdToken, string? ClientId = null);
}
