namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

/// <summary>
///     Command resource for upgrading a subscription.
/// </summary>
public record UpgradeSubscriptionResource(string NewPlanId);