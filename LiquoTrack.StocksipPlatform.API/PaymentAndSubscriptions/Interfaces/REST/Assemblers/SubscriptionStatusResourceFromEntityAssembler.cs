using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

/// <summary>
///     Static class to convert a subscription status string to SubscriptionStatusResource.
/// </summary>
public class SubscriptionStatusResourceFromEntityAssembler
{
    /// <summary>
    ///     Method to convert a subscription status string to SubscriptionStatusResource.
    /// </summary>
    /// <param name="status">
    ///     The subscription status string to convert.
    /// </param>
    /// <returns>
    ///     A new instance of SubscriptionStatusResource representing the provided status.
    /// </returns>
    public static SubscriptionStatusResource ToResourceFromEntity(string status)
    {
        return new SubscriptionStatusResource(status);
    }
}