using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

/// <summary>
///     Command service interface for handling subscription-related commands.
/// </summary>
public interface ISubscriptionsCommandService
{
    /// <summary>
    ///     Method to handle the activation of a paid subscription.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for activating a paid subscription.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the activated subscription.
    /// </returns>
    Task<(string?, string?)> Handle(InitialSubscriptionCommand command);

    /// <summary>
    ///     Method to handle webhook payment notifications.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for handling a webhook payment notification.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the updated subscription.
    /// </returns>
    Task<Subscription?> Handle(WebhookPaymentCommand command);
    
    /// <summary>
    ///     Method to handle the upgrade of a subscription.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for upgrading a subscription.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the upgraded subscription.
    /// </returns>
    Task<(string?, string?)> Handle(UpgradeSubscriptionCommand command);
}