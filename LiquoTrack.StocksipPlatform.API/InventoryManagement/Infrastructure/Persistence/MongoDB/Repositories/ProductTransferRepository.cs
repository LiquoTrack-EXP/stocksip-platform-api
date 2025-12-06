using System.Globalization;
using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Repository for managing ProductTransfer entities.
/// </summary>
public class ProductTransferRepository(AppDbContext context, IMediator mediator) 
    : BaseRepository<ProductTransfer>(context, mediator), IProductTransferRepository 
{
    // The MongoDB collection for the ProductTransfer aggregate.  
    private readonly IMongoCollection<ProductTransfer> _productTransferCollection = context.GetCollection<ProductTransfer>();
    
    /// <summary>
    ///     Method to retrieve all product transfers for a given warehouse ID.
    /// </summary>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to retrieve product transfers for.
    /// </param>
    /// <returns>
    ///     A list of product transfers for the specified warehouse, or an empty list if no transfers are found.
    /// </returns>
    public async Task<IEnumerable<ProductTransfer>> GetAllByWarehouseIdAsync(ObjectId warehouseId)
    {
        if (string.IsNullOrWhiteSpace(warehouseId.ToString()))
            throw new ArgumentException("ProductId cannot be null or empty.", nameof(warehouseId));
        
        return await _productTransferCollection
            .Find(x => x.OriginWarehouseId == warehouseId.ToString())
            .ToListAsync();
    }

    /// <summary>
    ///     Method to retrieve all product transfers for a given product ID.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to retrieve product transfers for.
    /// </param>
    /// <returns>
    ///     A list of product transfers for the specified product, or an empty list if no transfers are found.
    /// </returns>
    public async Task<IEnumerable<ProductTransfer>> GetAllByProductIdAsync(ObjectId productId)
    {
        if (string.IsNullOrWhiteSpace(productId.ToString()))
            throw new ArgumentException("ProductId cannot be null or empty.", nameof(productId));
        
        return await _productTransferCollection
            .Find(x => x.ProductId == productId.ToString())
            .ToListAsync();
    }
    
    /// <summary>
    ///     Method to retrieve a product transfer by product ID, warehouse ID, and expiration date (if aplicable).
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to retrieve a transfer for.
    /// </param>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to retrieve a transfer for.
    /// </param>
    /// <param name="expirationDate">
    ///     Optional. The expiration date of the transfer to retrieve.
    /// </param>
    /// <returns>
    ///     The product transfer if found; otherwise, null.
    /// </returns>
    public async Task<ProductTransfer?> GetByProductIdWarehouseIdAsync(ObjectId productId, ObjectId warehouseId, DateTime? expirationDate)
    {
        if (productId == ObjectId.Empty) 
            throw new ArgumentException("ProductId cannot be null or empty.", nameof(productId));
        
        if (warehouseId == ObjectId.Empty) 
            throw new ArgumentException("WarehouseId cannot be null or empty.", nameof(warehouseId));
        
        var stringProductId = productId.ToString();
        var stringWarehouseId = warehouseId.ToString();
        
        var filterBuilder = Builders<ProductTransfer>.Filter;
        var filter = filterBuilder.Eq(x => x.ProductId, stringProductId) &
                     filterBuilder.Eq(x => x.OriginWarehouseId, stringWarehouseId);
        
        if (expirationDate.HasValue)
        {
            filter = filterBuilder.Eq(x => x.ProductId, stringProductId) &
                     filterBuilder.Eq(x => x.OriginWarehouseId, stringWarehouseId) &
                     filterBuilder.Eq(x => x.ExpirationDate, expirationDate.Value.ToString(CultureInfo.CurrentCulture));
        }
        
        return await _productTransferCollection
            .Find(filter)
            .FirstOrDefaultAsync();
    }
}