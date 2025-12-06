namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Resource class for decreasing products from a warehouse.
/// </summary>
public record DecreaseProductsFromWarehouseResource(
    int QuantityToDecrease,
    string ExitType,
    DateTime? ExpirationDate
);