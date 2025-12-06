using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.Internal.QueryServices;

/// <summary>
/// Service implementation for handling catalog queries.
/// </summary>
public class CatalogQueryService(ICatalogRepository catalogRepository) : ICatalogQueryService
{
    /// <summary>
    /// Handles the GetCatalogByIdQuery to retrieve a catalog by its identifier.
    /// </summary>
    /// <param name="query">The query containing the catalog identifier.</param>
    /// <returns>The catalog if found, otherwise null.</returns>
    public async Task<Catalog?> Handle(GetCatalogByIdQuery query)
    {
        var catalogId = new CatalogId(query.catalogId);
        return await catalogRepository.GetByIdAsync(catalogId);
    }

    /// <summary>
    /// Handles the GetAllCatalogsQuery to retrieve all catalogs.
    /// </summary>
    /// <param name="query">The query to get all catalogs.</param>
    /// <returns>A collection of catalogs.</returns>
    public async Task<IEnumerable<Catalog>> Handle(GetAllCatalogsQuery query)
    {
        return await catalogRepository.GetAllAsync();
    }

    /// <summary>
    /// Handles the GetPublishedCatalogsQuery to retrieve all published catalogs.
    /// </summary>
    /// <param name="query">The query to get published catalogs.</param>
    /// <returns>A collection of published catalogs.</returns>
    public async Task<IEnumerable<Catalog>> Handle(GetPublishedCatalogsQuery query)
    {
        return await catalogRepository.FindPublishedAsync();
    }

    /// <summary>
    /// Handles the GetCatalogsByOwnerQuery to retrieve catalogs owned by a specific account.
    /// </summary>
    /// <param name="query">The query containing the owner account identifier.</param>
    /// <returns>A collection of catalogs owned by the account.</returns>
    public async Task<IEnumerable<Catalog>> Handle(GetCatalogsByOwnerQuery query)
    {
        var ownerId = new AccountId(query.ownerAccount);
        return await catalogRepository.FindByOwnerAsync(ownerId);
    }
}