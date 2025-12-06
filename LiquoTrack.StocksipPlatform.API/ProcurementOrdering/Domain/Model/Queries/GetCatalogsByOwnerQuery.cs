namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;

/// <summary>
/// Query to get all catalogs owned by a specific account.
/// </summary>
/// <param name="ownerAccount">The account identifier of the owner.</param>
public record GetCatalogsByOwnerQuery(string ownerAccount);