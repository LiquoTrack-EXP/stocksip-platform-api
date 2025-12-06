using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing a subscription.
/// </summary>
public partial class Subscription(
    string accountId,
    string planId
    ) : Entity
{
    /// <summary>
    ///     The Account Id associated with the subscription.
    /// </summary>
    public string AccountId { get; set; } = accountId;
    
    /// <summary>
    ///     The Plan Id associated with the subscription.
    /// </summary>
    public string PlanId { get; set; } = planId;
    
    /// <summary>
    ///     The status of the subscription.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public ESubscriptionStatus Status { get; set; }
    
    /// <summary>
    ///     Method to activate a trial subscription for a given premium plan.
    /// </summary>
    /// <param name="premiumPLan">
    ///     The premium plan to activate the trial for.
    /// </param>
    public void ActivateTrial(Plan premiumPLan)
    {
        PlanId = premiumPLan.Id.ToString();
        Status = ESubscriptionStatus.Trial;
        ExpirationDate = DateTime.UtcNow.AddDays(14);
    }
    
    /// <summary>
    ///     Method to activate a free subscription for a given free plan.   
    /// </summary>
    /// <param name="freePlan">
    ///     Free plan to activate the subscription for.
    /// </param>
    public void ActivateFreePlan(Plan freePlan)
    {
        PlanId = freePlan.Id.ToString();
        Status = ESubscriptionStatus.Active;
        ExpirationDate = CalculateExpirationDate(freePlan);
    }
    
    /// <summary>
    ///     Method to activate a premium subscription for a given premium plan.  
    /// </summary>
    /// <param name="premiumPlan">
    ///     Premium plan to activate the subscription for. 
    /// </param>
    public void ActivatePremiumPlan(Plan premiumPlan)
    {
        PlanId = premiumPlan.Id.ToString();
        Status = ESubscriptionStatus.Active;
        ExpirationDate = CalculateExpirationDate(premiumPlan);
    }

    /// <summary>
    ///     Method to activate a paid subscription for a given paid plan.
    /// </summary>
    /// <param name="paidPlan">
    ///     The paid plan to activate the subscription for.
    /// </param>
    public void ActivateEnterprisePlan(Plan paidPlan)
    {
        PlanId = paidPlan.Id.ToString();
        Status = ESubscriptionStatus.Active;
        ExpirationDate = CalculateExpirationDate(paidPlan);
    }

    /// <summary>
    ///     Method to calculate the expiration date based on the plan's payment frequency.
    /// </summary>
    /// <param name="plan">
    ///     The plan to calculate the expiration date for.
    /// </param>
    /// <returns>
    ///     A <see cref="DateTime"/> representing the calculated expiration date.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     An exception thrown when the payment frequency is not recognized.
    /// </exception>
    public DateTime CalculateExpirationDate(Plan plan)
    {
        return plan.PaymentFrequency switch
        {
            EPaymentFrequency.None => DateTime.MaxValue,
            EPaymentFrequency.Monthly => DateTime.UtcNow.AddMonths(1),
            EPaymentFrequency.Yearly => DateTime.UtcNow.AddYears(1),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}