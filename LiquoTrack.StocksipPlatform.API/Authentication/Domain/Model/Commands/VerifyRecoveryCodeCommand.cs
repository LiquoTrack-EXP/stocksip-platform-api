namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;

/// <summary>
///     Command to verify a recovery code.
/// </summary>
/// <param name="Email">
///     The email of the user.
/// </param>
/// <param name="RecoveryCode">
///     The recovery code to verify.
/// </param>
public record VerifyRecoveryCodeCommand(string Email, string RecoveryCode);