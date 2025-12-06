namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Settings;

/// <summary>
/// This class is used to store the Google authentication settings.
/// It is used to configure the Google authentication settings in the app settings .json file.
/// </summary>
public class GoogleAuthSettings
{
    /// <summary>
    /// Primary client ID used for audience validation (typically the Web/Backend client ID).
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Additional audiences allowed (e.g., Android OAuth client IDs, iOS, etc.).
    /// </summary>
    public List<string> AdditionalAudiences { get; set; } = new();
}
