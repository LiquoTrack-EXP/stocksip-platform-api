using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource representing an account with its associated business data.
/// </summary>
/// <param name="id">The unique identifier of the account.</param>
/// <param name="business">The business linked to the account.</param>
public record AccountWithBusinessResource(
    string id,
    BusinessResource business
);