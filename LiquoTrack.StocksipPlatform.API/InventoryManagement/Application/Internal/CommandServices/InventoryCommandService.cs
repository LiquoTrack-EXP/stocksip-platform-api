using Cortex.Mediator.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;

/// <summary>
///     Service to handle inventory commands
/// </summary>
/// <param name="inventoryRepository">
///     The repository for handling the Inventories in the database.
/// </param>
public class InventoryCommandService(
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IInventoryRepository inventoryRepository,
        IProductExitRepository productExitRepository,
        IProductTransferRepository productTransferRepository
    ) : IInventoryCommandService
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
    public async Task<Inventory?> Handle(AddProductsToWarehouseCommand command)
    {
        // Validate if the product exists
        var product = await productRepository.FindByIdAsync(command.ProductId.ToString())
            ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Validate if the warehouse exists
        var warehouse = await warehouseRepository.FindByIdAsync(command.WarehouseId.ToString())
            ?? throw new ArgumentException($"Warehouse with ID {command.WarehouseId} does not exist.");

        // Validate if the expiration date is provided
        if (command.ExpirationDate == null)
        {
            throw new ArgumentException("Expiration date is required when adding products with expiration date.");
        }

        // Validate if the inventory already exists
        var inventory = await inventoryRepository.GetByProductIdWarehouseIdAndExpirationDateAsync(command.ProductId,
                command.WarehouseId, command.ExpirationDate);

        // When the inventory does not exist, create it
        if (inventory == null)
        {
            var productStock = new ProductStock(command.QuantityToAdd);
            var newInventory = new Inventory(command.ProductId, command.WarehouseId, productStock, command.ExpirationDate);
            await inventoryRepository.AddAsync(newInventory);
            return newInventory;
        }
        
        // When the inventory exists, add the products to it
        inventory.AddStockToProduct(command.QuantityToAdd, product.MinimumStock.GetValue());
        
        // Updates the product 'totalStockInWarehouse' field
        var productToUpdate = await productRepository.FindByIdAsync(command.ProductId.ToString()) 
                              ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        productToUpdate.UpdateTotalStockInStore(productToUpdate.GetStockInStorage() + command.QuantityToAdd);
        
        await productRepository.UpdateAsync(productToUpdate.Id.ToString(), productToUpdate);
        await inventoryRepository.UpdateAsync(inventory.Id.ToString(), inventory);
        return inventory;
    }

    /// <summary>
    ///     Method to handle the addition of products to a warehouse without an expiration date. 
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for adding products to a warehouse.
    /// </param>
    /// <returns>
    ///     The inventory object representing the updated inventory or null if the inventory could not be updated.
    /// </returns>
    public async Task<Inventory?> Handle(AddProductsToWarehouseWithoutExpirationDateCommand command)
    {
        // Validate if the product exists
        var product = await productRepository.FindByIdAsync(command.ProductId.ToString())
                      ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Validate if the warehouse exists
        var warehouse = await warehouseRepository.FindByIdAsync(command.WarehouseId.ToString())
                        ?? throw new ArgumentException($"Warehouse with ID {command.WarehouseId} does not exist.");
        
        // Validate if the inventory already exists
        var inventory = await inventoryRepository.GetByProductIdWarehouseIdAsync(command.ProductId, command.WarehouseId);

        // When the inventory does not exist, create it
        if (inventory == null)
        {
            var productStock = new ProductStock(command.QuantityToAdd);
            var newInventory = new Inventory(command.ProductId, command.WarehouseId, productStock);
            await inventoryRepository.AddAsync(newInventory);
            return newInventory;
        }
        
        // When the inventory exists, add the products to it
        inventory.AddStockToProduct(command.QuantityToAdd, product.MinimumStock.GetValue());
        
        // Updates the product 'totalStockInWarehouse' field
        var productToUpdate = await productRepository.FindByIdAsync(command.ProductId.ToString()) 
                              ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Updates the product 'totalStockInWarehouse' field
        productToUpdate.UpdateTotalStockInStore(productToUpdate.GetStockInStorage() + command.QuantityToAdd);
        
        // Updates the product in the repository.
        await productRepository.UpdateAsync(productToUpdate.Id.ToString(), productToUpdate);
        
        // Updates the inventory in the repository.
        await inventoryRepository.UpdateAsync(inventory.Id.ToString(), inventory);
        
        // Returns the updated inventory.
        return inventory;
    }

    /// <summary>
    ///     Method to handle the removal of products from a warehouse.   
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for removing products from a warehouse. 
    /// </param>
    /// <returns>
    ///     The updated inventory.
    /// </returns>
    public async Task<Inventory?> Handle(DecreaseProductsFromWarehouseCommand command)
    {
        // Checks if the product exists.
        var product = await productRepository.FindByIdAsync(command.ProductId.ToString())
                      ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Checks if the warehouse exists.
        var warehouse = await warehouseRepository.FindByIdAsync(command.WarehouseId.ToString()) 
                        ?? throw new ArgumentException($"Warehouse with ID {command.WarehouseId} does not exist.");
        
        // Checks if the inventory exists.
        var inventoryToUpdate = await inventoryRepository.GetByProductIdWarehouseIdAndExpirationDateAsync(command.ProductId, command.WarehouseId, command.ExpirationDate)
            ?? throw new ArgumentException($"Inventory with product ID {command.ProductId} and warehouse ID {command.WarehouseId} does not exist.");
        
        // Decreases the stock of the product in the inventory.
        inventoryToUpdate.DecreaseStockFromProduct(command.QuantityToDecrease, product.MinimumStock.GetValue(), warehouse.AccountId);
        
        // Updates the product 'totalStockInWarehouse' field
        var productToUpdate = await productRepository.FindByIdAsync(command.ProductId.ToString()) 
                              ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Creates a new product exit record
        var productExit = new ProductExit(
            productToUpdate.Id.ToString(),
            productToUpdate.Name,
            warehouse.Id.ToString(),
            warehouse.Name,
            command.ExitType,
            command.QuantityToDecrease,
            inventoryToUpdate.GetStock() + command.QuantityToDecrease,
            command.ExpirationDate.ToString()
        );
        
        // Updates the product 'totalStockInWarehouse' field
        productToUpdate.UpdateTotalStockInStore(productToUpdate.GetStockInStorage() - command.QuantityToDecrease);

        // Updates the product exit record
        await productExitRepository.AddAsync(productExit);
        
        // Updates the product in the repository.
        await productRepository.UpdateAsync(productToUpdate.Id.ToString(), productToUpdate);
        
        // Updates the inventory in the repository.
        await inventoryRepository.UpdateAsync(inventoryToUpdate.Id.ToString(), inventoryToUpdate);
        
        // Publishes the events related to the inventory.
        await inventoryRepository.PublishEventsAsync(inventoryToUpdate);
        
        // Returns the updated inventory.
        return inventoryToUpdate;
    }

    /// <summary>
    ///     Method to handle the removal of products from a warehouse without an expiration date.  
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for removing products from a warehouse.
    /// </param>
    /// <returns>
    ///     The inventory object representing the updated inventory or null if the inventory could not be updated.
    /// </returns>
    public async Task<Inventory?> Handle(DecreaseProductsFromWarehouseWithoutExpirationDateCommand command)
    {
        // Checks if the product exists.
        var product = await productRepository.FindByIdAsync(command.ProductId.ToString())
                      ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Checks if the warehouse exists.
        var warehouse = await warehouseRepository.FindByIdAsync(command.WarehouseId.ToString()) 
                        ?? throw new ArgumentException($"Warehouse with ID {command.WarehouseId} does not exist.");
        
        // Checks if the inventory exists.
        var inventoryToUpdate = await inventoryRepository.GetByProductIdWarehouseIdAsync(command.ProductId, command.WarehouseId)
                                ?? throw new ArgumentException($"Inventory with product ID {command.ProductId} and warehouse ID {command.WarehouseId} does not exist.");
        
        // Decreases the stock of the product in the inventory.
        inventoryToUpdate.DecreaseStockFromProduct(command.QuantityToDecrease, product.MinimumStock.GetValue(), warehouse.AccountId);
        
        // Updates the product 'totalStockInWarehouse' field
        var productToUpdate = await productRepository.FindByIdAsync(command.ProductId.ToString()) 
                              ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Creates a new product exit record
        var productExit = new ProductExit(
            productToUpdate.Id.ToString(),
            productToUpdate.Name,
            warehouse.Id.ToString(),
            warehouse.Name,
            command.ExitType,
            command.QuantityToDecrease,
            inventoryToUpdate.GetStock() + command.QuantityToDecrease
        );
        
        // Updates the product 'totalStockInWarehouse' field
        productToUpdate.UpdateTotalStockInStore(productToUpdate.GetStockInStorage() - command.QuantityToDecrease);
        
        // Updates the product exit record
        await productExitRepository.AddAsync(productExit);
        
        // Updates the product in the repository.
        await productRepository.UpdateAsync(productToUpdate.Id.ToString(), productToUpdate);
        
        // Updates the inventory in the repository.
        await inventoryRepository.UpdateAsync(inventoryToUpdate.Id.ToString(), inventoryToUpdate);
        
        // Publishes the events related to the inventory.
        await inventoryRepository.PublishEventsAsync(inventoryToUpdate);
        
        // Returns the updated inventory.
        return inventoryToUpdate;
    }

    /// <summary>
    ///     Method to handle the transfer of products from one warehouse to another.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for transferring products from one warehouse to another.
    /// </param>
    /// <returns>
    ///     The updated inventory or null if the inventory could not be updated.
    /// </returns>
    public async Task<Inventory?> Handle(TransferProductsToAnotherWarehouseCommand command)
    {
        // Validate if the product to be moved exists.
        var movedProduct = await productRepository.FindByIdAsync(command.ProductId.ToString())
                           ?? throw new ArgumentException($"Product with ID {command.ProductId} does not exist.");
        
        // Validate if the original warehouse exists.
        var originWarehouse = await warehouseRepository.FindByIdAsync(command.OriginWarehouseId.ToString()) 
                            ?? throw new ArgumentException($"Warehouse with ID {command.OriginWarehouseId} does not exist.");
        
        // Validate if the new warehouse where the product will be moved exists.
        var newWarehouse = await warehouseRepository.FindByIdAsync(command.DestinationWarehouseId.ToString())
                           ?? throw new ArgumentException($"Warehouse with ID {command.DestinationWarehouseId} does not exist.");
        
        // Validate if the old warehouse where the product will be moved exists.
        if (command.DestinationWarehouseId == command.OriginWarehouseId)
        {
            throw new ArgumentException("Cannot move products to the same warehouse.");
        }

        // Initializes a new inventory object with the new warehouse and the moved stock expiration date.
        Inventory currentInventory;
        
        if (command.ExpirationDate == null)
        {
            // Retrieves the current inventory of the product in the old warehouse.
            currentInventory = 
                await inventoryRepository.GetByProductIdWarehouseIdAsync(command.ProductId, command.OriginWarehouseId) 
                                   ?? throw new ArgumentException($"Inventory with Product ID {command.ProductId} and Warehouse ID {command.OriginWarehouseId} does not exist.");
        }
        else
        {
            // Retrieves the current inventory of the product in the old warehouse with the specified expiration date.
            currentInventory =
                await inventoryRepository.GetByProductIdWarehouseIdAndExpirationDateAsync(command.ProductId,
                    command.OriginWarehouseId, new ProductExpirationDate(DateOnly.FromDateTime(command.ExpirationDate.Value)))
                                    ?? throw new ArgumentException($"Inventory with Product ID {command.ProductId} and Warehouse ID {command.OriginWarehouseId} does not exist.");
        }
        
        // Removes the moved stock from the current inventory. And If the current inventory has no stock left, the product state will be set to OUT_OF_STOCK.
        currentInventory.DecreaseStockFromProduct(command.QuantityToTransfer, movedProduct.MinimumStock.GetValue(), newWarehouse.AccountId);

        // Updates the inventory in the repository.
        await inventoryRepository.UpdateAsync(currentInventory.Id.ToString(), currentInventory);
        
        // Publishes the events related to the inventory.
        await inventoryRepository.PublishEventsAsync(currentInventory);
        
        // Initializes a new inventory object with the new warehouse and the moved stock expiration date.
        Inventory destinationInventory;
        
        // Validates the expiration date of the moved product.
        if (command.ExpirationDate == null) {
            
            // Retrieves the destination inventory of the product in the new warehouse.
            destinationInventory = await inventoryRepository.GetByProductIdWarehouseIdAsync(command.ProductId, command.DestinationWarehouseId)
                                   ?? new Inventory(command.ProductId, command.DestinationWarehouseId, new ProductStock(0), new ProductExpirationDate());
        }
        else {
            
            // Retrieves the destination inventory of the product in the new warehouse with the specified expiration date.
            var expiration = new ProductExpirationDate(DateOnly.FromDateTime(command.ExpirationDate.Value));
            destinationInventory = await inventoryRepository.GetByProductIdWarehouseIdAndExpirationDateAsync(command.ProductId, command.DestinationWarehouseId, expiration)
                                   ?? new Inventory(command.ProductId, command.DestinationWarehouseId, new ProductStock(0), expiration);
        }
        
        // Adds the moved stock to the destination inventory.
        destinationInventory.AddStockToProduct(command.QuantityToTransfer, movedProduct.MinimumStock.GetValue());

        // Updates the inventory in the repository.
        if (destinationInventory.Id == ObjectId.Empty) {
            // If the destination inventory does not exist, creates it.
            await inventoryRepository.AddAsync(destinationInventory);
        } else {
            // If the destination inventory exists, updates it.
            await inventoryRepository.UpdateAsync(destinationInventory.Id.ToString(), destinationInventory);
        }

        // Creates a new product transfer record.
        var transferRecord = new ProductTransfer(
            movedProduct.Id.ToString(),
            movedProduct.Name,
            originWarehouse.Id.ToString(),
            originWarehouse.Name,
            newWarehouse.Id.ToString(),
            newWarehouse.Name,
            command.QuantityToTransfer,
            currentInventory.GetStock() - command.QuantityToTransfer,
            destinationInventory.GetStock()
        );
        
        // Adds the product transfer record to the repository.
        await productTransferRepository.AddAsync(transferRecord);
        
        // Publishes the events related to the destination inventory.
        await inventoryRepository.PublishEventsAsync(destinationInventory);

        // Returns the updated/current inventory.
        return currentInventory;
    }

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
    public async Task Handle(DeleteInventoryCommand command)
    {
        // Verifies that the inventory exists.
        var inventoryToDelete = await inventoryRepository.FindByIdAsync(command.InventoryId.ToString())
                                ?? throw new ArgumentException($"Inventory with ID {command.InventoryId} does not exist.");
        
        // Deletes the inventory from the repository.
        await inventoryRepository.DeleteAsync(inventoryToDelete.Id.ToString());
    }
}
