namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Request resource for transferring products to another warehouse
/// </summary>
public record TransferProductsToAnotherWarehouseResource(string DestinationWarehouseId, int QuantityToTransfer, DateTime? ExpirationDate);