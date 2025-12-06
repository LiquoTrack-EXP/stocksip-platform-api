namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

/// <summary>
///     Resource for registering a new sub-user.
/// </summary>
public record RegisterSubUserResource(string Name, string Email, string Password, string Role, string PhoneNumber, string ProfileRole);