using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Repository implementation for the Product aggregate. 
/// </summary>
public class ProductRepository(AppDbContext context, IMediator mediator) : BaseRepository<Product>(context, mediator), IProductRepository
{
    /// <summary>
    ///     The MongoDB collection for the Product aggregate.   
    /// </summary>
    private readonly IMongoCollection<Product> _productCollection = context.GetCollection<Product>();
    
    /// <summary>
    ///     Method to check if a product exists by a given name.
    /// </summary>
    /// <param name="name">
    ///     The name of the product to check for existence.
    /// </param>
    /// <param name="accountId">
    ///     The ID of the account to check for existence.
    /// </param>
    /// <returns>
    ///     True if a product with the specified name exists; otherwise, false.
    /// </returns>
    public async Task<bool> ExistsByNameAndAccountIdAsync(ProductName name, AccountId accountId)
    {
        return await _productCollection
            .Find(x => 
                x.Name == name.GetValue() && 
                x.AccountId.GetId == accountId.GetId)
            .AnyAsync();
    }

    /// <summary>
    ///     Method to check if a product exists by a given ID.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to check for existence.
    /// </param>
    /// <returns>
    ///     True if a product with the specified ID exists; otherwise, false.
    /// </returns>
    public async Task<bool> ExistsByIdAsync(ObjectId productId)
    {
        return await _productCollection
            .Find(x => x.Id == productId)
            .AnyAsync();
    }

    /// <summary>
    ///     Method to find products by a given supplier ID.
    /// </summary>
    /// <param name="supplierId">
    ///     The ID of the supplier to find products for.
    /// </param>
    /// <returns>
    ///     A list of products for the specified supplier.
    ///     Or an empty list if no products are found.
    /// </returns>
    public async Task<ICollection<Product>> FindBySupplierIdAsync(AccountId supplierId)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.SupplierId, supplierId);
        return await _productCollection.Find(filter).ToListAsync();
    }

    /// <summary>
    ///     Method to find products by a given warehouse ID.
    /// </summary>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to find products for.
    /// </param>
    /// <returns>
    ///     A list of products for the specified warehouse.
    ///     Or an empty list if no products are found.
    /// </returns>
    public Task<ICollection<Product>> FindByWarehouseIdAsync(ObjectId warehouseId)
    {
        throw new NotImplementedException();
        
        // TODO: Complete this when doing inventories section for doing this
        
        // var productIds = await _inventoryCollection
        //  .Find(inv => inv.WarehouseId == warehouseId)
        //  .Project(inv => inv.ProductId)
        //  .Distinct()
        //  .ToListAsync();
        
        // var products = await _productCollection
        //  .Find(prod => productIds.Contains(prod.Id))
        //  .ToListAsync();
    }

    /// <summary>
    ///     Method to find products by a given account ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account to find products for.
    /// </param>
    /// <returns>
    ///     The list of products associated with the specified account ID.
    /// </returns>
    public async Task<ICollection<Product>> FindByAccountIdAsync(AccountId accountId)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.AccountId, accountId);
        return await _productCollection.Find(filter).ToListAsync();
    }

    public async Task<string> FindImageUrlByProductIdAsync(ObjectId productId)
    {
        var imageUrlValueObject = await _productCollection
            .Find(x => x.Id == productId)
            .Project(p => p.ImageUrl)
            .FirstOrDefaultAsync();
        
        return imageUrlValueObject?.GetValue() ?? string.Empty;
    }

    /// <summary>
    ///     This method counts the number of products associated with a specific account ID.
    /// </summary>
    /// <param name="accountId">
    ///     The unique identifier of the account.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing the count of products.
    /// </returns>
    public Task<int> CountByAccountIdAsync(AccountId accountId)
    {
        var filter = Builders<Product>.Filter.Eq(w => w.AccountId, accountId);
        return _productCollection.CountDocumentsAsync(filter).ContinueWith(t => (int)t.Result);
    }
}