namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;

public record SalesOrderItemResource(
    string ProductId,
    string ProductName,
    decimal UnitPrice,
    string Currency,
    string? InventoryId,
    int QuantityToSell
);
