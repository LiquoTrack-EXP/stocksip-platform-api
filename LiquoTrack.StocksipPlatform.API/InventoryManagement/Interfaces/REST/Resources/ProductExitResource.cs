namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Record representing a resource for product exit.
/// </summary>
public record ProductExitResource(
    string ProductId, 
    string ProductName, 
    string WarehouseId,
    string? ExpirationDate,
    string WarehouseName, 
    string ExitType,
    int OutputQuantity,
    int PreviousQuantity,
    DateTime ExitDate
);