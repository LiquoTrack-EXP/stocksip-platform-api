namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

/// <summary>
///     Message resource for sending a code to recover a password.
/// </summary>
/// <param name="Email"></param>
public record SendCodeToRecoverPasswordResource(string Email);