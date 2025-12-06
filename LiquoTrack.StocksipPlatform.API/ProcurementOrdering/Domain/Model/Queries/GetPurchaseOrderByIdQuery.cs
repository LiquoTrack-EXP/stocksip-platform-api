namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;

/// <summary>
/// Query to get a purchase order by its identifier.
/// </summary>
/// <param name="orderId">The identifier of the purchase order.</param>
public record GetPurchaseOrderByIdQuery(string orderId);