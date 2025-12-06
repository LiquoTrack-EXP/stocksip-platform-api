namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;

/// <summary>
///     Query to retrieve subscriptions by account ID.
/// </summary>
/// <param name="AccountId">
///     The unique identifier of the account whose subscriptions are to be retrieved.
/// </param>
public record GetSubscriptionByAccountIdQuery(string AccountId);