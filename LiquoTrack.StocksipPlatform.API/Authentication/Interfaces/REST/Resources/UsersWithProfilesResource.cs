namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a user along with their profile information.
/// </summary>
public record UsersWithProfilesResource(
    string UserId,
    string Email,
    string Role,
    string? ProfileId,
    string? FullName,
    string? PhoneNumber,
    string? ProfilePictureUrl,
    string? profileRole
);