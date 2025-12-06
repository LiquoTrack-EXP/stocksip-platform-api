namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource representing a purchase order.
/// </summary>
/// <param name="id">The unique identifier of the purchase order.</param>
/// <param name="orderCode">The order code.</param>
/// <param name="items">The list of items in the order.</param>
/// <param name="status">The current status of the order.</param>
/// <param name="catalogIdBuyFrom">The identifier of the catalog.</param>
/// <param name="generationDate">The date when the order was generated.</param>
/// <param name="confirmationDate">The date when the order was confirmed.</param>
/// <param name="buyer">The buyer account identifier.</param>
/// <param name="isOrderSent">Indicates if the order has been sent.</param>
/// <param name="total">The total amount of the order.</param>
public record PurchaseOrderResource(
    string id,
    string orderCode,
    List<PurchaseOrderItemResource> items,
    string status,
    string catalogIdBuyFrom,
    DateTime generationDate,
    DateTime? confirmationDate,
    string buyer,
    bool isOrderSent,
    decimal total
);