namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

/// <summary>
///     Resource for resetting a user's password.'
/// </summary>
public record ResetPasswordResource(string Email, string NewPassword);