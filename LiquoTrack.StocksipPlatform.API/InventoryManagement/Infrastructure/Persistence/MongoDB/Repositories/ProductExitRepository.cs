using System.Globalization;
using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Implementation of the ProductExitRepository interface.
/// </summary>
public class ProductExitRepository(AppDbContext context, IMediator mediator)
    : BaseRepository<ProductExit>(context, mediator), IProductExitRepository
{
    /// <summary>
    ///     Collection for the ProductExit entity.
    /// </summary>
    private readonly IMongoCollection<ProductExit> _productExitCollection = context.GetCollection<ProductExit>();

    /// <summary>
    ///     Retrieves all product exits for a given warehouse ID.
    /// </summary>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to retrieve product exits for.
    /// </param>
    /// <returns>
    ///     A list of product exits for the specified warehouse or a blank list if no product exits are found.
    /// </returns>
    public async Task<IEnumerable<ProductExit>> GetAllByWarehouseIdAsync(ObjectId warehouseId)
    {
        if (warehouseId == ObjectId.Empty) 
            throw new ArgumentException("WarehouseId cannot be null or empty.", nameof(warehouseId));
        
        var stringWarehouseId = warehouseId.ToString();
        
        return await _productExitCollection
            .Find(x => x.WarehouseId == stringWarehouseId)
            .ToListAsync();
    }

    /// <summary>
    ///     Retrieves all product exits for a given product ID.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to retrieve product exits for.
    /// </param>
    /// <returns>
    ///     A list of product exits for the specified product or a blank list if no product exits are found.
    /// </returns>
    public async Task<IEnumerable<ProductExit>> GetAllByProductIdAsync(ObjectId productId)
    {
        if (productId == ObjectId.Empty) 
            throw new ArgumentException("ProductId cannot be null or empty.", nameof(productId));
        
        var stringProductId = productId.ToString();
        
        return await _productExitCollection
            .Find(x => x.ProductId == stringProductId)
            .ToListAsync();
    }

    /// <summary>
    ///     Retrieves a product exit by product ID and warehouse ID.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product.
    /// </param>
    /// <param name="warehouseId">
    ///     The ID of the warehouse.
    /// </param>
    /// <param name="expirationDate">
    ///     The expiration date of the product exit.
    /// </param>
    /// <returns>
    ///     The product exit if found; otherwise, null.
    /// </returns>
    public async Task<ProductExit?> GetByProductIdAndWarehouseIdAsync(ObjectId productId, ObjectId warehouseId, DateTime? expirationDate)
    {
        if (productId == ObjectId.Empty) 
            throw new ArgumentException("ProductId cannot be null or empty.", nameof(productId));
        
        if (warehouseId == ObjectId.Empty) 
            throw new ArgumentException("WarehouseId cannot be null or empty.", nameof(warehouseId));
        
        var stringProductId = productId.ToString();
        var stringWarehouseId = warehouseId.ToString();
        
        var filterBuilder = Builders<ProductExit>.Filter;
        var filter = filterBuilder.Eq(x => x.ProductId, stringProductId) &
                     filterBuilder.Eq(x => x.WarehouseId, stringWarehouseId);
        
        if (expirationDate.HasValue)
        {
            filter = filterBuilder.Eq(x => x.ProductId, stringProductId) &
                     filterBuilder.Eq(x => x.WarehouseId, stringWarehouseId) &
                     filterBuilder.Eq(x => x.ExpirationDate, expirationDate.Value.ToString(CultureInfo.CurrentCulture));
        }
        
        return await _productExitCollection
            .Find(filter)
            .FirstOrDefaultAsync();
    }
}