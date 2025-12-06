namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to create a new purchase order.
/// </summary>
/// <param name="orderCode">The unique code for the purchase order.</param>
/// <param name="catalogIdBuyFrom">The identifier of the catalog to buy from.</param>
/// <param name="buyer">The account identifier of the buyer.</param>
/// <param name="addressIndex">The address of the buyer business.</param>
public record CreatePurchaseOrderCommand(
    string orderCode,
    string catalogIdBuyFrom,
    string buyer,
    int? addressIndex = null
);