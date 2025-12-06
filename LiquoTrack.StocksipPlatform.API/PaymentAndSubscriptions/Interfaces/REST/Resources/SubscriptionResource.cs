namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

/// <summary>
///     Resource class for Subscriptions.
/// </summary>
public record SubscriptionResource(string SubscriptionId, string PlanId, string Status, string ExpirationDate, string PlanType, string PaymentFrequency, int MaxUsers, int MaxProducts, int MaxWarehouses);