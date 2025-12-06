namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.Jobs.Hosted;

/// <summary>
///     Interface for the service that checks for expired subscriptions.
/// </summary>
public interface ISubscriptionsExpirationService
{
    /// <summary>
    ///     Method to check for expired subscriptions.
    /// </summary>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task CheckPendingSubscriptionsAsync();
}