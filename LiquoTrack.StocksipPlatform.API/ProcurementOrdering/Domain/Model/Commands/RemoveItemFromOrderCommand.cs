namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to remove an item from a purchase order.
/// </summary>
/// <param name="orderId">The identifier of the purchase order.</param>
/// <param name="productId">The identifier of the product to remove.</param>
public record RemoveItemFromOrderCommand(string orderId, string productId);