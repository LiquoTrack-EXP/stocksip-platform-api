namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;

/// <summary>
///     Query to retrieve the status of an account by its ID.
/// </summary>
/// <param name="AccountId">
///     The unique identifier of the account whose status is to be retrieved.
/// </param>
public record GetAccountStatusByIdQuery(string AccountId);