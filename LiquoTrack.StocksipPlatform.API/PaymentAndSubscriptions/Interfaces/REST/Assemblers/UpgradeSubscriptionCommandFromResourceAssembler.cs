using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

/// <summary>
///     Static class to convert UpgradeSubscriptionResource to UpgradeSubscriptionCommand.
/// </summary>
public class UpgradeSubscriptionCommandFromResourceAssembler
{
    /// <summary>
    ///     Method to convert UpgradeSubscriptionResource to UpgradeSubscriptionCommand.
    /// </summary>
    /// <param name="accountId">
    ///     The account id of the account to upgrade.
    /// </param>
    /// <param name="subscriptionId">
    ///     Subscription to upgrade. 
    /// </param>   
    /// <param name="resource">
    ///     Resource to convert.
    /// </param>
    /// <returns>
    ///     A new instance of UpgradeSubscriptionCommand representing the provided resource.   
    /// </returns>
    public static UpgradeSubscriptionCommand ToCommandFromResource(string accountId, string subscriptionId, UpgradeSubscriptionResource resource)
        => new UpgradeSubscriptionCommand(accountId, subscriptionId, resource.NewPlanId);
}