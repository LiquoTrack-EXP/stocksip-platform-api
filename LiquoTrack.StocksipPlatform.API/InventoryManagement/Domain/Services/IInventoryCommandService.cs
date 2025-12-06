using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

/// <summary>
///     Interface for handling inventory-related commands.
/// </summary>
public interface IInventoryCommandService
{
    /// <summary>
    ///     Method to handle the addition of products to a warehouse.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for adding products to a warehouse.
    /// </param>
    /// <returns>
    ///     An inventory object representing the updated inventory.
    /// </returns>
    Task<Inventory?> Handle(AddProductsToWarehouseCommand command);

    /// <summary>
    ///     Method to handle the addition of products to a warehouse without an expiration date. 
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for adding products to a warehouse.
    /// </param>
    /// <returns>
    ///     The inventory object representing the updated inventory or null if the inventory could not be updated.
    /// </returns>
    Task<Inventory?> Handle(AddProductsToWarehouseWithoutExpirationDateCommand command);
    
    /// <summary>
    ///     Method to handle the removal of products from a warehouse.   
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for removing products from a warehouse. 
    /// </param>
    /// <returns>
    ///     The updated inventory or null if the inventory could not be updated.
    /// </returns>
    Task<Inventory?> Handle(DecreaseProductsFromWarehouseCommand command);

    /// <summary>
    ///     Method to handle the removal of products from a warehouse without an expiration date.  
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for removing products from a warehouse.
    /// </param>
    /// <returns>
    ///     The inventory object representing the updated inventory or null if the inventory could not be updated.
    /// </returns>
    Task<Inventory?> Handle(DecreaseProductsFromWarehouseWithoutExpirationDateCommand command);

    /// <summary>
    ///     Method to handle the transfer of products from one warehouse to another.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for transferring products from one warehouse to another.
    /// </param>
    /// <returns>
    ///     The updated inventory or null if the inventory could not be updated.
    /// </returns>
    Task<Inventory?> Handle(TransferProductsToAnotherWarehouseCommand command);
    
    /// <summary>
    ///     Method to handle the deletion of an inventory.
    ///     Use this when the stock is zero and is no longer needed. 
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for deleting an inventory.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    Task Handle(DeleteInventoryCommand command);
}