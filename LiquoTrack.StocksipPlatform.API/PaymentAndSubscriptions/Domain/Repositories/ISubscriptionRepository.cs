using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;

/// <summary>
///     Repository interface for managing Subscription entities.
/// </summary>
public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    /// <summary>
    ///     Method to find an active subscription by an account ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account.
    /// </param>
    /// <returns>
    ///     A subscription if found; otherwise, null.   
    /// </returns>
    Task<Subscription?> FindActiveSubscriptionByAccountIdAsync(string accountId);
    
    /// <summary>
    ///     Method to find a pending subscription by preference ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account.
    /// </param>
    /// <returns>
    ///     A subscription if found; otherwise, null.
    /// </returns>
    Task<Subscription?> FindPendingSubscriptionByAccountIdIdAsync(string accountId);
    
    /// <summary>
    ///     Method to find a subscription by its preference ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account.
    /// </param>
    /// <returns>
    ///     A subscription if found; otherwise, null.
    /// </returns>
    Task<Subscription?> FindByAccountIdAsync(string accountId);
    
    /// <summary>
    ///     Method to find a subscription status by preference ID.
    /// </summary>
    /// <param name="preferenceId">
    ///     The ID of the preference.
    /// </param>
    /// <returns>
    ///     A string representing the status of the subscription.
    /// </returns>
    Task<string?> FindSubscriptionStatusByPreferenceId(string preferenceId);
    
    /// <summary>
    ///     Method to find all pending updates for an account.
    /// </summary>
    /// <returns>
    ///     A list of subscriptions.
    /// </returns>
    Task<IEnumerable<Subscription?>> FindAllPendingUpdateAsync();
} 