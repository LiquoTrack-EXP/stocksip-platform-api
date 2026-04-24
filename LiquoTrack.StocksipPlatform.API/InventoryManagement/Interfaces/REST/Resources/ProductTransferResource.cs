namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Resource for a product transfer.
/// </summary>
public record ProductTransferResource(
    string TransferId,
    string ProductId, 
    string ProductName, 
    string OriginWarehouseId, 
    string OriginWarehouseName,
    string DestinationWarehouseId,
    string DestinationWarehouseName,
    int TransferredStock, 
    int OriginWarehouseRemainingStock,
    int DestinationWarehouseResultingStock,
    DateTime TransferDate,
    string? ExpirationDate
);