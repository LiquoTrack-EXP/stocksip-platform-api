namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;

/// <summary>
///     Command to reset a password
/// </summary>
/// <param name="Email">
///     The email of the user.
/// </param>
/// <param name="NewPassword">
///     The new password to set.
/// </param>
public record ResetPasswordCommand(string Email, string NewPassword);