using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to remove an item from a catalog.
/// </summary>
/// <param name="catalogId">The identifier of the catalog.</param>
/// <param name="productId">The identifier of the product to remove.</param>
public record RemoveItemFromCatalogCommand(string catalogId, ProductId productId);