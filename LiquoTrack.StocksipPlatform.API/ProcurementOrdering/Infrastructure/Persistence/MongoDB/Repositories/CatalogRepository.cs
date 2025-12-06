using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
/// Repository for managing Catalog entities.
/// </summary>
public class CatalogRepository(AppDbContext context, IMediator mediator)
    : BaseRepository<Catalog>(context, mediator), ICatalogRepository
{
    /// <summary>
    /// The MongoDB collection for the Catalog entity.
    /// </summary>
    private readonly IMongoCollection<Catalog> _catalogCollection = context.GetCollection<Catalog>();

    /// <summary>
    /// Gets a catalog by its identifier.
    /// </summary>
    /// <param name="id">The catalog identifier.</param>
    /// <returns>The catalog if found, otherwise null.</returns>
    public async Task<Catalog?> GetByIdAsync(CatalogId id)
    {
        if (string.IsNullOrWhiteSpace(id.GetId()))
            throw new ArgumentException("CatalogId cannot be null or empty.", nameof(id));

        return await _catalogCollection
            .Find(c => c.Id.ToString() == id.GetId())
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets all published catalogs.
    /// </summary>
    /// <returns>A collection of published catalogs.</returns>
    public async Task<IEnumerable<Catalog>> FindPublishedAsync()
    {
        return await _catalogCollection
            .Find(c => c.IsPublished)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all catalogs owned by a specific account.
    /// </summary>
    /// <param name="ownerAccount">The owner account identifier.</param>
    /// <returns>A collection of catalogs owned by the account.</returns>
    public async Task<IEnumerable<Catalog>> FindByOwnerAsync(AccountId ownerAccount)
    {
        if (string.IsNullOrWhiteSpace(ownerAccount.GetId))
            throw new ArgumentException("Owner AccountId cannot be null or empty.", nameof(ownerAccount));

        var filter = Builders<Catalog>.Filter.Eq("ownerAccount", ownerAccount.GetId);
        return await _catalogCollection.Find(filter).ToListAsync();
    }

    /// <summary>
    /// Checks if a catalog exists by its name and owner.
    /// </summary>
    /// <param name="name">The catalog name.</param>
    /// <param name="ownerAccount">The owner account identifier.</param>
    /// <returns>True if the catalog exists, otherwise false.</returns>
    public async Task<bool> ExistsByNameAndOwnerAsync(string name, AccountId ownerAccount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Catalog name cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(ownerAccount.GetId))
            throw new ArgumentException("Owner AccountId cannot be null or empty.", nameof(ownerAccount));

        return await _catalogCollection
            .Find(c => c.Name == name && c.OwnerAccount.GetId == ownerAccount.GetId)
            .AnyAsync();
    }
}