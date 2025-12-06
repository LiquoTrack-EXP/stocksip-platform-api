namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;

/// <summary>
///     Query to get the plan products limit by account id.
/// </summary>
/// <param name="AccountId">
///     The ID of the account.
/// </param>
public record GetPlanProductsLimitByAccountIdQuery(string AccountId);