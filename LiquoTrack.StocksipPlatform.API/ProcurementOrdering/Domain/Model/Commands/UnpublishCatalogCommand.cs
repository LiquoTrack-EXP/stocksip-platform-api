namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to unpublish a catalog, making it invisible to buyers.
/// </summary>
/// <param name="catalogId">The identifier of the catalog to unpublish.</param>
public record UnpublishCatalogCommand(string catalogId);