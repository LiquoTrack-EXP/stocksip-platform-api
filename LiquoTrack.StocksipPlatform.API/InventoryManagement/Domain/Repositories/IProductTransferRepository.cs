using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;

/// <summary>
///     Interface for the ProductTransfer repository.
/// </summary>
public interface IProductTransferRepository : IBaseRepository<ProductTransfer>
{
    /// <summary>
    ///     Method to retrieve all product transfers for a given warehouse ID.
    /// </summary>
    /// <param name="warehouseId">
    ///     The ID of the warehouse to retrieve product transfers for.
    /// </param>
    /// <returns>
    ///     A list of product transfers for the specified warehouse, or an empty list if no transfers are found.
    /// </returns>
    Task<IEnumerable<ProductTransfer>> GetAllByWarehouseIdAsync(ObjectId warehouseId);
    
    /// <summary>
    ///     Method to retrieve all product transfers for a given product ID.
    /// </summary>
    /// <param name="productId">
    ///     The ID of the product to retrieve product transfers for.
    /// </param>
    /// <returns>
    ///     A list of product transfers for the specified product, or an empty list if no transfers are found.
    /// </returns>
    Task<IEnumerable<ProductTransfer>> GetAllByProductIdAsync(ObjectId productId);
    
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
    Task<ProductTransfer?> GetByProductIdWarehouseIdAsync(ObjectId productId, ObjectId warehouseId, DateTime? expirationDate);
}