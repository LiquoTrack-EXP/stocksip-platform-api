namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;

/// <summary>
///     Query to retrieve the number of users allowed for a given plan.
/// </summary>
/// <param name="accountId">
///     The unique identifier of the account for which the number of users is to be retrieved.
/// </param>
public record GetPlanUsersLimitByAccountIdQuery(string accountId);