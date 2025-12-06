using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Repository for managing Subscription entities
/// </summary>
public class SubscriptionRepository(AppDbContext context, IMediator mediator) : BaseRepository<Subscription> (context, mediator), ISubscriptionRepository
{
    /// <summary>
    ///     The MongoDB collection for subscriptions.
    /// </summary>
    private readonly IMongoCollection<Subscription> _subscriptionCollection = context.GetCollection<Subscription>();
    
    /// <summary>
    ///     Method to find an active subscription by an account ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account.
    /// </param>
    /// <returns>
    ///     A subscription if found; otherwise, null.  
    /// </returns>
    public async Task<Subscription?> FindActiveSubscriptionByAccountIdAsync(string accountId)
    {
        var subscription = await _subscriptionCollection
            .Find(s => s.AccountId == accountId && s.Status == ESubscriptionStatus.Active)
            .FirstOrDefaultAsync();

        return subscription;
    }

    /// <summary>
    ///     Method to find a pending subscription by an account ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account.
    /// </param>
    /// <returns>
    ///     A subscription if found; otherwise, null.
    /// </returns>
    public async Task<Subscription?> FindPendingSubscriptionByAccountIdIdAsync(string accountId)
    {
        var subscription = await _subscriptionCollection
            .Find(s => s.AccountId == accountId && s.Status == ESubscriptionStatus.PendingPayment)
            .FirstOrDefaultAsync();

        return subscription;
    }

    /// <summary>
    ///      Method to find a subscription by its preference ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account.
    /// </param>
    /// <returns>
    ///     A subscription if found; otherwise, null. 
    /// </returns>
    public async Task<Subscription?> FindByAccountIdAsync(string accountId)
    {
        var subscription = await _subscriptionCollection
            .Find(s => s.AccountId == accountId)
            .FirstOrDefaultAsync();
        
        return subscription;
    }

    /// <summary>
    ///     Method to find a subscription status by preference ID.
    /// </summary>
    /// <param name="preferenceId">
    ///     The ID of the preference.
    /// </param>
    /// <returns>
    ///     A string representing the status of the subscription.
    /// </returns>
    public async Task<string?> FindSubscriptionStatusByPreferenceId(string preferenceId)
    {
        return await _subscriptionCollection
            .Find(s => s.PreferenceId == preferenceId)
            .Project(s => s.Status.ToString())
            .FirstOrDefaultAsync();
    }

    /// <summary>
    ///     Method to find all pending subscriptions for upgrade.
    /// </summary>
    /// <returns>
    ///     A list of subscriptions. 
    /// </returns>
    public async Task<IEnumerable<Subscription?>> FindAllPendingUpdateAsync()
    {
        return await _subscriptionCollection
            .Find(s => s.Status == ESubscriptionStatus.PendingUpgradePayment)
            .ToListAsync();
    }
}