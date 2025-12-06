namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;

/// <summary>
/// Query to get a catalog by its identifier.
/// </summary>
/// <param name="catalogId">The identifier of the catalog.</param>
public record GetCatalogByIdQuery(string catalogId);