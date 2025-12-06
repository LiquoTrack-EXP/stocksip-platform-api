namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

public record CatalogItemResource(
    string ProductId,
    decimal Amount,
    string Currency,
    DateTime AddedDate,
    string? ProductName,
    string? ProductImage,
    int? AvailableStock
);