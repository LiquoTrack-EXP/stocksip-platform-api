using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;

/// <summary>
/// Interface for catalog query service.
/// </summary>
public interface ICatalogQueryService
{
    /// <summary>
    /// Handles the GetCatalogByIdQuery.
    /// </summary>
    /// <param name="query">The query to get a catalog by id.</param>
    /// <returns>The catalog if found, otherwise null.</returns>
    Task<Catalog?> Handle(GetCatalogByIdQuery query);

    /// <summary>
    /// Handles the GetAllCatalogsQuery.
    /// </summary>
    /// <param name="query">The query to get all catalogs.</param>
    /// <returns>A collection of catalogs.</returns>
    Task<IEnumerable<Catalog>> Handle(GetAllCatalogsQuery query);

    /// <summary>
    /// Handles the GetPublishedCatalogsQuery.
    /// </summary>
    /// <param name="query">The query to get published catalogs.</param>
    /// <returns>A collection of published catalogs.</returns>
    Task<IEnumerable<Catalog>> Handle(GetPublishedCatalogsQuery query);

    /// <summary>
    /// Handles the GetCatalogsByOwnerQuery.
    /// </summary>
    /// <param name="query">The query to get catalogs by owner.</param>
    /// <returns>A collection of catalogs owned by the account.</returns>
    Task<IEnumerable<Catalog>> Handle(GetCatalogsByOwnerQuery query);
}