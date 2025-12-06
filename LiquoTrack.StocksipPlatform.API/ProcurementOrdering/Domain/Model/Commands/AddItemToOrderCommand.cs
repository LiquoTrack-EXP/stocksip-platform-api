namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to add an item to an existing purchase order.
/// </summary>
/// <param name="OrderId">The identifier of the purchase order.</param>
/// <param name="ProductId">The identifier of the product to add.</param>
/// <param name="Quantity">The quantity of the product to add.</param>
public record AddItemToOrderCommand(
    string OrderId,
    string ProductId,
    int Quantity
);