using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     The MongoDB repository implementation for managing CareGuide entities.
/// </summary>
public class CareGuideRepository : BaseRepository<CareGuide>, ICareGuideRepository
{
    /// <summary>
    ///     The MongoDB collection for the CareGuide aggregate.   
    /// </summary>
    private readonly IMongoCollection<CareGuide> _careGuideCollection;
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="CareGuideRepository"/> class.
    /// </summary>
    public CareGuideRepository(AppDbContext context, IMediator mediator) : base(context, mediator)
    {
        _careGuideCollection = context.GetCollection<CareGuide>();
    }

    /// <summary>
    ///     Gets a care guide by its ID.
    /// </summary>
    /// <param name="id">The ID of the care guide to retrieve.</param>
    /// <returns>The care guide if found; otherwise, null.</returns>
    public async Task<CareGuide?> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;
        if (ObjectId.TryParse(id, out var objectId))
        {
            var byMongoId = Builders<CareGuide>.Filter.Eq("_id", objectId);
            var foundByMongoId = await _careGuideCollection.Find(byMongoId).FirstOrDefaultAsync();
            if (foundByMongoId is not null) return foundByMongoId;
        }
        var byDomainId = Builders<CareGuide>.Filter.Eq(x => x.CareGuideId, id);
        return await _careGuideCollection.Find(byDomainId).FirstOrDefaultAsync();
    }

    /// <summary>   
    ///     Gets all care guides by account ID.
    /// </summary>
    /// <param name="accountId">The account ID to find care guides for.</param>
    /// <returns>A collection of care guides for the specified account.</returns>
    public async Task<IEnumerable<CareGuide>> GetAllByAccountId(string accountId)
    {
        if (string.IsNullOrEmpty(accountId))
            return Enumerable.Empty<CareGuide>();
        // Use direct field filter to avoid LINQ translation issues with value objects
        var filter = Builders<CareGuide>.Filter.Eq("accountId", accountId);
        return await _careGuideCollection.Find(filter).ToListAsync();
    }
    
    /// <summary>
    ///     Updates a care guide using its domain CareGuideId (GUID string).
    /// </summary>
    /// <param name="careGuideId">Domain care guide identifier (e.g., GUID string).</param>
    /// <param name="entity">The care guide entity with updated values.</param>
    public async Task UpdateByCareGuideIdAsync(string careGuideId, CareGuide entity)
    {
        if (string.IsNullOrWhiteSpace(careGuideId))
            throw new ArgumentException("careGuideId is required", nameof(careGuideId));

        entity.UpdatedAt = System.DateTime.UtcNow;

        var result = await _careGuideCollection.ReplaceOneAsync(x => x.CareGuideId == careGuideId, entity);
        if (result.MatchedCount == 0)
            throw new KeyNotFoundException($"CareGuide '{careGuideId}' not found");
    }
    /// <summary>
    /// Gets all care guides by product ID.
    /// <param name="productId">The product ID to find care guides for.</param>
    /// <returns>A collection of care guides for the specified product.</returns>
    public async Task<IEnumerable<CareGuide>> GetAllByProductId(string productId)
    {
        if (string.IsNullOrWhiteSpace(productId))
            return Enumerable.Empty<CareGuide>();

        var hasMongoId = ObjectId.TryParse(productId, out var productObjectId);

        var filter = hasMongoId
            ? Builders<CareGuide>.Filter.Or(
                Builders<CareGuide>.Filter.Where(x => x.ProductAssociated != null && x.ProductAssociated.Id == productObjectId),
                Builders<CareGuide>.Filter.Eq(x => x.ProductId, productId)
              )
            : Builders<CareGuide>.Filter.Eq(x => x.ProductId, productId);

        return await _careGuideCollection.Find(filter).ToListAsync();
    }
    /// <param name="accountId">The account ID to find the care guide for.</param>
    /// <param name="productType">The product type to find the care guide for.</param>
    /// <returns>The care guide if found; otherwise, null.</returns>
    public async Task<CareGuide?> GetByProductType(string accountId, string productType)
    {
        if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(productType))
            return null;

        var accountIdObj = AccountId.Create(accountId);
        var normalizedProductType = productType.Trim();

        var careGuide = await _careGuideCollection
            .Find(cg => cg.AccountId == accountIdObj &&
                        cg.ProductId != null &&
                        cg.ProductId.Trim().Equals(normalizedProductType, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefaultAsync();

        if (careGuide != null)
            return careGuide;

        return await _careGuideCollection
            .Find(cg => cg.AccountId == accountIdObj &&
                       (cg.ProductId != null &&
                        (cg.ProductId.Trim().Equals(normalizedProductType, StringComparison.OrdinalIgnoreCase) ||
                         cg.ProductId.Trim().Equals(normalizedProductType.ToLower()) ||
                         cg.ProductId.Trim().Equals(normalizedProductType.ToUpper()))))
            .FirstOrDefaultAsync();
    }

    /// <summary>
    ///     Method to check if a care guide exists by a given ID.
    /// </summary>
    /// <param name="id">The ID of the care guide to check for existence.</param>
    /// <returns>True if a care guide with the specified ID exists; otherwise, false.</returns>
    public async Task<bool> ExistsByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return false;
            
        return await _careGuideCollection
            .Find(x => x.Id == objectId)
            .AnyAsync();
    }
}
