using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

public record ReduceCatalogItemStockCommand(
    string CatalogId,
    ProductId ProductId,
    int Quantity
);