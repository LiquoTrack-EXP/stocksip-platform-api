namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Record that represents a product resource.
/// </summary>
public record ProductResource(
        string Id,
        string Name,
        string Type,
        string Brand,
        decimal UnitPrice,
        string Code,
        int MinimumStock,
        int TotalStockInStore,
        decimal Content,
        string ImageUrl,
        string AccountId,
        string SupplierId,
        bool IsInWarehouse
    );