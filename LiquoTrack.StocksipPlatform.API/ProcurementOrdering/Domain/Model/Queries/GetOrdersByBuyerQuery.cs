namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;

/// <summary>
/// Query to get all purchase orders for a specific buyer.
/// </summary>
/// <param name="buyer">The account identifier of the buyer.</param>
public record GetOrdersByBuyerQuery(string buyer);