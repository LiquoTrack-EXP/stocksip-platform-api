using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

/// <summary>
///     Static class to convert Subscription entity to SubscriptionResource.
/// </summary>
public class SubscriptionResourceFromEntityAssembler
{
    /// <summary>
    ///     Method to convert Subscription entity to SubscriptionResource.
    /// </summary>
    /// <param name="subscription">
    ///     Subscription entity to convert.
    /// </param>
    /// <param name="plan">
    ///     Subscription plan to convert.
    /// </param>
    /// <returns>
    ///     A new instance of SubscriptionResource representing the provided entity. 
    /// </returns>
    public static SubscriptionResource ToResourceFromEntity(Subscription subscription, Plan plan)
        => new SubscriptionResource(
            subscription.Id.ToString(), 
            subscription.PlanId, 
            subscription.Status.ToString(), 
            subscription.ExpirationDate.ToShortDateString(),
            plan.PlanType.ToString(),
            plan.PaymentFrequency.ToString(),
            plan.PlanLimits.MaxUsers,
            plan.PlanLimits.MaxProducts,
            plan.PlanLimits.MaxWarehouses
            );
}