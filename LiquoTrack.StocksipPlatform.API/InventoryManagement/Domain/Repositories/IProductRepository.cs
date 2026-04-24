using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;

/// <summary>
///     Repository interface for managing Product entities.
/// </summary>
public interface IProductRepository : IBaseRepository<Product>
{
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
    Task<bool> ExistsByNameAndAccountIdAsync(ProductName name, AccountId accountId);

    /// <summary>
    ///     Method to check if a product exists by a given ID.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to check for existence.
    /// </param>
    /// <returns>
    ///     True if a product with the specified ID exists; otherwise, false.
    /// </returns>
    Task<bool> ExistsByIdAsync(ObjectId productId);
    
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
    Task<ICollection<Product>> FindBySupplierIdAsync(AccountId supplierId);

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
    Task<ICollection<Product>> FindByWarehouseIdAsync(ObjectId warehouseId);

    /// <summary>
    ///     Method to find products by a given account ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account to find products for.
    /// </param>
    /// <returns>
    ///     The list of products associated with the specified account ID.
    /// </returns>
    Task<ICollection<Product>> FindByAccountIdAsync(AccountId accountId);
    
    /// <summary>
    ///     Method to find the image URL associated with a product.   
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product. 
    /// </param>
    /// <returns>
    ///     A string containing the image URL.
    /// </returns>
    Task<string> FindImageUrlByProductIdAsync(ObjectId productId);
    
    /// <summary>
    ///     This method counts the number of products associated with a specific account ID.
    /// </summary>
    /// <param name="accountId">
    ///     The unique identifier of the account.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing the count of products.
    /// </returns>
    Task<int> CountByAccountIdAsync(AccountId accountId);
}