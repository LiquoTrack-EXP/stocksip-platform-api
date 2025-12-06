namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource for adding an item to a purchase order using only the product ID from the catalog.
/// Quantity is optional and defaults to 1 if not provided.
/// </summary>
/// <param name="productId">The identifier of the product to add.</param>
/// <param name="quantity">Optional quantity to purchase (default is 1).</param>
public record AddItemToOrderResource(
    string productId,
    int? quantity = 1
);