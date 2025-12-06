namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a subscription.
/// </summary>
public record PaymentPreferenceResource(string PreferenceId, string InitPoint, string Message);