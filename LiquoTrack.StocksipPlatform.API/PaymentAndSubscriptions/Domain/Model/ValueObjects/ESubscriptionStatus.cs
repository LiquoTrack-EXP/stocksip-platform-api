namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the status of a subscription.
/// </summary>
public enum ESubscriptionStatus
{
    Trial,
    Active,
    PastDue,
    Canceled,
    Expired,
    PendingPayment,
    PendingUpgradePayment
}