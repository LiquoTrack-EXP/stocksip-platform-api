namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;

/// <summary>
/// Query to get all purchase orders from a specific catalog.
/// </summary>
/// <param name="catalogId">The identifier of the catalog.</param>
public record GetOrdersByCatalogQuery(string catalogId);