using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Repository for managing Inventory entities.
/// </summary>
public class InventoryRepository(AppDbContext context, IMediator mediator)
    : BaseRepository<Inventory>(context, mediator), IInventoryRepository
{
    /// <summary>
    ///     The MongoDB collection for the Brand entity.   
    /// </summary>
    private readonly IMongoCollection<Inventory> _inventoryCollection = context.GetCollection<Inventory>();
    
    /// <summary>
    ///     Method to get an inventory item by product ID, warehouse ID and expiration date.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to find inventory items for.
    /// </param>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to find inventory items for.
    /// </param>
    /// <param name="expirationDate">
    ///     The expiration date of the inventory item to find.
    /// </param>
    /// <returns>
    ///     An inventory item if found; otherwise, null.
    /// </returns>
    public async Task<Inventory?> GetByProductIdWarehouseIdAndExpirationDateAsync(
        ObjectId productId, ObjectId warehouseId, ProductExpirationDate expirationDate)
    {
        var expirationDateValue = expirationDate.GetValue();

        var filter = Builders<Inventory>.Filter.And(
            Builders<Inventory>.Filter.Eq(x => x.ProductId, productId),
            Builders<Inventory>.Filter.Eq(x => x.WarehouseId, warehouseId),
            Builders<Inventory>.Filter.Eq("ExpirationDate.Value", expirationDateValue)
        );

        return await _inventoryCollection.Find(filter).FirstOrDefaultAsync();
    }



    /// <summary>
    ///     Method to get an inventory item by product ID and warehouse ID.
    ///     Used when the inventory does not have an expiration date.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to find inventory items for.
    /// </param>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to find inventory items for.
    /// </param>
    /// <returns>
    ///     An inventory item if found; otherwise, null.
    /// </returns>
    public async Task<Inventory?> GetByProductIdWarehouseIdAsync(ObjectId productId, ObjectId warehouseId)
    {
        var filter = Builders<Inventory>.Filter.And(
            Builders<Inventory>.Filter.Eq(x => x.ProductId, productId),
            Builders<Inventory>.Filter.Eq(x => x.WarehouseId, warehouseId),
            Builders<Inventory>.Filter.Exists(x => x.ExpirationDate, false)
        );

        return await _inventoryCollection.Find(filter).FirstOrDefaultAsync();

    }

    /// <summary>
    ///     Method to check if an inventory item exists by product ID, warehouse ID and expiration date.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to find inventory items for.
    /// </param>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to find inventory items for.
    /// </param>
    /// <param name="expirationDate">
    ///     The expiration date of the inventory item to find.
    /// </param>
    /// <returns>
    ///     A boolean indicating whether an inventory item exists.
    /// </returns>
    public async Task<bool> ExistsByProductIdWarehouseIdAndExpirationDateAsync(ObjectId productId, ObjectId warehouseId,
        ProductExpirationDate expirationDate)
    {
        if (string.IsNullOrWhiteSpace(warehouseId.ToString()) && string.IsNullOrWhiteSpace(productId.ToString()))
            throw new ArgumentException("WarehouseId or ProductId cannot be null or empty.", nameof(warehouseId));
        
        return await _inventoryCollection
            .Find(x => x.ProductId == productId 
                       && x.WarehouseId == warehouseId 
                       && x.ExpirationDate == expirationDate)
            .AnyAsync();
    }

    /// <summary>
    ///     Method to check if an inventory item exists by product ID, warehouse ID and without expiration date.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to find inventory items for.
    /// </param>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to find inventory items for.
    /// </param>
    /// <returns>
    ///     A boolean indicating whether an inventory item exists.
    /// </returns>
    public async Task<bool> ExistsByProductIdWarehouseIdAsync(ObjectId productId, ObjectId warehouseId)
    {
        if (string.IsNullOrWhiteSpace(warehouseId.ToString()) && string.IsNullOrWhiteSpace(productId.ToString()))
            throw new ArgumentException("WarehouseId or ProductId cannot be null or empty.", nameof(warehouseId));
        
        return await _inventoryCollection
            .Find(x => x.ProductId == productId 
                       && x.WarehouseId == warehouseId 
                       && x.ExpirationDate == null)
            .AnyAsync();
    }

    /// <summary>
    ///     Method to check if an inventory item has an expiration date.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to find inventory items for.
    /// </param>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to find inventory items for.
    /// </param>
    /// <returns>
    ///     A boolean indicating whether an inventory item has an expiration date.
    /// </returns>
    public async Task<bool> HasExpirationDateAsync(ObjectId productId, ObjectId warehouseId)
    {
        var filter = Builders<Inventory>.Filter.And(
            Builders<Inventory>.Filter.Eq(x => x.ProductId, productId),
            Builders<Inventory>.Filter.Eq(x => x.WarehouseId, warehouseId),
            Builders<Inventory>.Filter.Exists(x => x.ExpirationDate, true)
        );

        return await _inventoryCollection.Find(filter).AnyAsync();
    }

    /// <summary>
    ///     Method to find all inventory items by product ID.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to find inventory items for. 
    /// </param>
    /// <returns>
    ///     A list of inventory items for the specified product.
    /// </returns>
    public async Task<IEnumerable<Inventory>> FindByProductIdAsync(ObjectId productId)
    {
        if (string.IsNullOrWhiteSpace(productId.ToString()))
            throw new ArgumentException("ProductId cannot be null or empty.", nameof(productId));
        
        return await _inventoryCollection
            .Find(x => x.ProductId == productId)
            .ToListAsync();
    }

    /// <summary>
    ///     Method to find all inventory items by warehouse ID.   
    /// </summary>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to find inventory items for.
    /// </param>
    /// <returns>
    ///     A list of inventory items for the specified warehouse.
    /// </returns>
    public async Task<IEnumerable<Inventory>> FindByWarehouseIdAsync(ObjectId warehouseId)
    {
        if (string.IsNullOrWhiteSpace(warehouseId.ToString()))
            throw new ArgumentException("WarehouseId cannot be null or empty.", nameof(warehouseId));
        
        return await _inventoryCollection
            .Find(x => x.WarehouseId == warehouseId)
            .ToListAsync();
    }
}