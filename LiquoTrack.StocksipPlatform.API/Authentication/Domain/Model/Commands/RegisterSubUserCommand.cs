namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;

/// <summary>
///     Command for registering a new sub-user in the system.
/// </summary>
public record RegisterSubUserCommand(string Name, string Email, string Password, string Role, string PhoneNumber, string AccountId, string ProfileRole);