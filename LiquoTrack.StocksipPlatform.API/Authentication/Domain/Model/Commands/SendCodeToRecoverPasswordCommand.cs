namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;

/// <summary>
///     Command to send a code to recover a password.
/// </summary>
/// <param name="Email">
///     The email of the user.
/// </param>
public record SendCodeToRecoverPasswordCommand(string Email);