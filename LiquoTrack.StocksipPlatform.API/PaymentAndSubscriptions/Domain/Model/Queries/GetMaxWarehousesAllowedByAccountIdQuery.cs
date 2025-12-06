namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;

/// <summary>
///     Method to retrieve the maximum number of warehouses allowed for a given account.
/// </summary>
/// <param name="AccountId">
///     The unique identifier of the account for which the maximum number of warehouses is to be retrieved.
/// </param>
public record GetPlanWarehouseLimitByAccountId(string AccountId);