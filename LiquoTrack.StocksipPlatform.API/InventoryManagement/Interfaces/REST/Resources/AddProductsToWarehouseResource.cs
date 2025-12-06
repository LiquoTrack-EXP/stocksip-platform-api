using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Resource class for adding products to a warehouse.
/// </summary>
public record AddProductsToWarehouseResource(
    int QuantityToAdd,
    DateTime? ExpirationDate
);