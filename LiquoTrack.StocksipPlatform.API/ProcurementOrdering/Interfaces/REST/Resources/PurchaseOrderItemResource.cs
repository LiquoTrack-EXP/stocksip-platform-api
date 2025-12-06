namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource representing an item in a purchase order.
/// </summary>
/// <param name="productId">The product identifier.</param>
/// <param name="unitPrice">The unit price of the product.</param>
/// <param name="quantity">The quantity ordered.</param>
/// <param name="subTotal">The subtotal for this item.</param>
public record PurchaseOrderItemResource(
    string productId,
    string productName,
    decimal unitPrice,
    int quantity,
    decimal subTotal
);