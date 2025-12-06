using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;

public partial class Subscription
{
    /// <summary>
    ///     The expiration date of the subscription.
    /// </summary>
    public DateTime ExpirationDate { get; set; }
    
    /// <summary>
    ///     The preference ID of the subscription.
    /// </summary>
    public string? PreferenceId { get; set; }
    
    /// <summary>
    ///     The pending plan ID of the subscription.
    /// </summary>
    public string? PendingPlanId { get; set; }
    
    /// <summary>
    ///     Method to mark the subscription as pending payment.
    /// </summary>
    /// <param name="plan">
    ///     The plan to subscribe to.
    /// </param>
    /// <param name="preferenceId">
    ///     The preference ID of the subscription.
    /// </param>
    public void MarkAsPending(Plan plan, string preferenceId)
    {
        PlanId = plan.Id.ToString();
        PreferenceId = preferenceId;
        Status = ESubscriptionStatus.PendingPayment;
        ExpirationDate = DateTime.UtcNow;
    }
    
    /// <summary>
    ///     Method to mark the subscription as pending upgrade.
    /// </summary>
    /// <param name="newPlanId">
    ///     The new plan to upgrade to.
    /// </param>   
    /// <param name="preferenceId">
    ///     The preference ID of the subscription.
    /// </param>
    public void MarkAsPendingUpgrade(string newPlanId, string preferenceId)
    {
        PendingPlanId = newPlanId;
        Status = ESubscriptionStatus.PendingUpgradePayment;
        PreferenceId = preferenceId;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    ///     Method to cancel the subscription.
    /// </summary>
    public void CancelSubscription()
    {
        Status = ESubscriptionStatus.Canceled;
        ExpirationDate = DateTime.UtcNow;
    }
    
    /// <summary>
    ///     Method to mark the subscription as expired.
    /// </summary>
    public void MarkAsExpired() => Status = ESubscriptionStatus.Expired;
    
    /// <summary>
    ///     Method to check if the subscription is pending payment.
    /// </summary>
    public bool IsPendingPayment() => Status == ESubscriptionStatus.PendingPayment;
    
    /// <summary>
    ///     Method to mark the subscription as cancelled.
    /// </summary>
    public void MarkAsCancelled() => Status = ESubscriptionStatus.Canceled;

    /// <summary>
    ///     Method to mark the subscription as cancelled.     
    /// </summary>
    public void MarkAsCancelledUpdate()
    {
        Status = ESubscriptionStatus.Active;
        PendingPlanId = null;
        PreferenceId = null;
    }

}