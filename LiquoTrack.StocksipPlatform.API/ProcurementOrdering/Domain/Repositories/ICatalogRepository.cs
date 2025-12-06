using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;

/// <summary>
/// Repository interface for catalog operations.
/// </summary>
public interface ICatalogRepository : IBaseRepository<Catalog>
{
    /// <summary>
    /// Gets a catalog by its identifier.
    /// </summary>
    /// <param name="id">The catalog identifier.</param>
    /// <returns>The catalog if found, otherwise null.</returns>
    Task<Catalog?> GetByIdAsync(CatalogId id);

    /// <summary>
    /// Gets all published catalogs.
    /// </summary>
    /// <returns>A collection of published catalogs.</returns>
    Task<IEnumerable<Catalog>> FindPublishedAsync();

    /// <summary>
    /// Gets all catalogs owned by a specific account.
    /// </summary>
    /// <param name="ownerAccount">The owner account identifier.</param>
    /// <returns>A collection of catalogs owned by the account.</returns>
    Task<IEnumerable<Catalog>> FindByOwnerAsync(AccountId ownerAccount);

    /// <summary>
    /// Checks if a catalog exists by its name and owner.
    /// </summary>
    /// <param name="name">The catalog name.</param>
    /// <param name="ownerAccount">The owner account identifier.</param>
    /// <returns>True if the catalog exists, otherwise false.</returns>
    Task<bool> ExistsByNameAndOwnerAsync(string name, AccountId ownerAccount);
}