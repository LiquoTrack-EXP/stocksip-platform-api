using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

/// <summary>
///     Static class to convert Subscription entity to SubscriptionResource.
/// </summary>
public class PaymentPreferenceResourceFromEntityAssembler
{
    /// <summary>
    ///     Method to convert Subscription entity to SubscriptionResource.
    /// </summary>
    /// <param name="preferenceId">}
    ///     The preference id of the subscription in MercadoPago.
    /// </param>
    /// <param name="initPoint">
    ///     The url to redirect to after the subscription is created.
    /// </param>
    /// <param name="message">
    ///     The message to display to the user.
    /// </param>
    /// <returns>
    ///     A SubscriptionResource object.
    /// </returns>
    public static PaymentPreferenceResource ToResourceFromEntity(string preferenceId, string initPoint, string message)
    {
        return new PaymentPreferenceResource(
            preferenceId,
            initPoint,
            message
            );
    }
}