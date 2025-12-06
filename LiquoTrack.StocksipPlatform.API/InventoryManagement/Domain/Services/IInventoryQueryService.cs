using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

/// <summary>
///     Interface for handling inventory-related queries.
/// </summary>
public interface IInventoryQueryService
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
    Task<Inventory?> Handle(GetInventoryByIdQuery query);
    
    /// <summary>
    ///     Method to handle the retrieval of all inventories associated with a specific product ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID for which inventories are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of inventories associated with the specified product ID.
    /// </returns>
    Task<IEnumerable<Inventory>> Handle(GetAllInventoriesByProductIdQuery query);
    
    /// <summary>
    ///     The method to handle the retrieval of all inventories associated with a specific warehouse ID.   
    /// </summary>
    /// <param name="query">
    ///     The query object containing the warehouse ID for which inventories are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of inventories associated with the specified warehouse ID.
    /// </returns>
    Task<IEnumerable<Inventory>> Handle(GetAllInventoriesByWarehouseIdQuery query);
    
    /// <summary>
    ///     The method to handle the retrieval of an inventory by product ID, warehouse ID, and expiration date. 
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID, warehouse ID, and expiration date for which an inventory is to be retrieved.
    /// </param>
    /// <returns>
    ///     The inventory if found; otherwise, null.
    /// </returns>
    Task<Inventory?> Handle(GetInventoryByProductIdWarehouseIdAndExpirationDateQuery query);

    /// <summary>
    ///     Method to handle the retrieval of an inventory by product ID and warehouse ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID and warehouse ID for which an inventory is to be retrieved.
    /// </param>
    /// <returns>
    ///     The inventory if found; otherwise, null.   
    /// </returns>
    Task<Inventory?> Handle(GetInventoryByProductIdAndWarehouseIdQuery query);
}