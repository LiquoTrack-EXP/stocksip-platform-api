using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.QueryServices;

/// <summary>
///     The service for handling inventory-related queries.
/// </summary>
/// <param name="inventoryRepository">
///     The repository for handling the Inventories in the database.
/// </param>
public class InventoryQueryService(
        IInventoryRepository inventoryRepository
    ) : IInventoryQueryService
{
    /// <summary>
    ///     Method to get an inventory by its ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the inventory ID.
    /// </param>
    /// <returns>
    ///     The inventory if found; otherwise, null.
    /// </returns>
    public async Task<Inventory?> Handle(GetInventoryByIdQuery query)
    {
        return await inventoryRepository.FindByIdAsync(query.InventoryId.ToString());
    }

    /// <summary>
    ///     Method to handle the retrieval of all inventories associated with a specific product ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID for which inventories are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of inventories associated with the specified product ID.
    /// </returns>
    public async Task<IEnumerable<Inventory>> Handle(GetAllInventoriesByProductIdQuery query)
    {
        return await inventoryRepository.FindByProductIdAsync(query.ProductId);
    }

    /// <summary>
    ///     The method to handle the retrieval of all inventories associated with a specific warehouse ID.   
    /// </summary>
    /// <param name="query">
    ///     The query object containing the warehouse ID for which inventories are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of inventories associated with the specified warehouse ID.
    /// </returns>
    public async Task<IEnumerable<Inventory>> Handle(GetAllInventoriesByWarehouseIdQuery query)
    {
        return await inventoryRepository.FindByWarehouseIdAsync(query.WarehouseId);
    }

    /// <summary>
    ///     The method to handle the retrieval of an inventory by product ID, warehouse ID, and expiration date. 
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID, warehouse ID, and expiration date for which an inventory is to be retrieved.
    /// </param>
    /// <returns>
    ///     The inventory if found; otherwise, null.
    /// </returns>
    public async Task<Inventory?> Handle(GetInventoryByProductIdWarehouseIdAndExpirationDateQuery query)
    {
        return await inventoryRepository.GetByProductIdWarehouseIdAndExpirationDateAsync(query.ProductId, query.WarehouseId, query.ExpirationDate);
    }

    /// <summary>
    ///     Method to handle the retrieval of an inventory by product ID and warehouse ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID and warehouse ID for which an inventory is to be retrieved.
    /// </param>
    /// <returns>
    ///     The inventory if found; otherwise, null.   
    /// </returns>
    public async Task<Inventory?> Handle(GetInventoryByProductIdAndWarehouseIdQuery query)
    {
        return await inventoryRepository.GetByProductIdWarehouseIdAsync(query.ProductId, query.WarehouseId);
    }
}