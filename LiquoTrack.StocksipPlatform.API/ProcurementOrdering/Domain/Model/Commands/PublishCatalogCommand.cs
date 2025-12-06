namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to publish a catalog, making it visible to buyers.
/// </summary>
/// <param name="catalogId">The identifier of the catalog to publish.</param>
public record PublishCatalogCommand(string catalogId);