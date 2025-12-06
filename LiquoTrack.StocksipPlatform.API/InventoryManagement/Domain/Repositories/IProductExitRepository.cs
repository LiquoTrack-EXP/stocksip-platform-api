using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;

/// <summary>
///     Interface for managing ProductExit entities.
/// </summary>
public interface IProductExitRepository : IBaseRepository<ProductExit>
{
    /// <summary>
    ///     Retrieves all product exits for a given warehouse ID.
    /// </summary>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to retrieve product exits for.
    /// </param>
    /// <returns>
    ///     A list of product exits for the specified warehouse or a blank list if no product exits are found.
    /// </returns>
    Task<IEnumerable<ProductExit>> GetAllByWarehouseIdAsync(ObjectId warehouseId);
    
    /// <summary>
    ///     Retrieves all product exits for a given product ID.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to retrieve product exits for.
    /// </param>
    /// <returns>
    ///     A list of product exits for the specified product or a blank list if no product exits are found.
    /// </returns>
    Task<IEnumerable<ProductExit>> GetAllByProductIdAsync(ObjectId productId);
    
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
    Task<ProductExit?> GetByProductIdAndWarehouseIdAsync(ObjectId productId, ObjectId warehouseId, DateTime? expirationDate);
}