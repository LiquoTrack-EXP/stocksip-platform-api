namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

/// <summary>
///     Resource for verifying a recovery code.
/// </summary>
public record VerifyRecoveryCodeResource(string Email, string Code);