using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.OutboundServices.FileStorage;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Exceptions;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;

/// <summary>
///     The service for handling warehouse-related commands.
/// </summary>
/// <param name="warehouseRepository">
///     The repository for handling the Warehouses in the database.
/// </param>
public class WarehouseCommandService(
        IWarehouseRepository warehouseRepository,
        IInventoryImageService inventoryImageService,
        IPaymentAndSubscriptionsFacade paymentAndSubscriptionsFacade
    ) : IWarehouseCommandService
{
    /// <summary>
    ///     This method handles the creation of a new warehouse based on the provided command.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details required to create a new warehouse.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing the created Warehouse object or null if creation failed.
    /// </returns>
    public async Task<Warehouse?> Handle(RegisterWarehouseCommand command)
    {
        // Verifies that the warehouse does not already exist with the same name.
        if (await warehouseRepository.ExistByNameIgnoreCaseAndAccountIdAsync(command.Name, command.AccountId)) 
            throw new WarehouseFailedCreationException($"Warehouse with name {command.Name} already exists.");
        
        // Verifies that the account has not reached the maximum number of warehouses.
        var currentWarehousesCount = await warehouseRepository.CountByAccountIdAsync(command.AccountId);
        
        var maxAllowedWarehouses = await paymentAndSubscriptionsFacade
            .GetPlanWarehouseLimitByAccountId(command.AccountId.GetId);
        
        if (maxAllowedWarehouses is not null && currentWarehousesCount >= maxAllowedWarehouses)
            throw new WarehouseFailedCreationException("The account has reached the maximum number of warehouses for the current plan.");
        
        // Gets the image url of the warehouse.
        string imageUrl = command.Image != null ? 
            inventoryImageService.UploadImage(command.Image) 
            : "https://res.cloudinary.com/deuy1pr9e/image/upload/v1759709826/Default-warehouse_qdgvkw.jpg"; 
        
        // Verifies that the warehouse does not already exist with the same address.
        if (await warehouseRepository.ExistsByStreetAndCityAndPostalCodeIgnoreCaseAndAccountIdAsync(
                command.Address.Street, command.Address.City, command.Address.PostalCode, command.AccountId))
        {
            throw new WarehouseFailedCreationException(
                $"Warehouse with address {command.Address.Street}, " +
                $"{command.Address.City}, " +
                $"{command.Address.PostalCode} " +
                $"already exists.");
        }
        
        // Create the warehouse.
        var warehouse = new Warehouse(command, imageUrl);

        // Tries to add the warehouse to the repository.
        try
        {
            await warehouseRepository.AddAsync(warehouse);
            return warehouse;
        }
        // If the warehouse could not be added, throws an exception.
        catch (WarehouseFailedCreationException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    ///     This method handles the update of an existing warehouse based on the provided command.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details required to update an existing warehouse.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing the updated Warehouse object or null if the update failed.
    /// </returns>
    public async Task<Warehouse?> Handle(UpdateWarehouseInformationCommand command)
    {
        // Verifies that the warehouse exists.
        var accountId = await warehouseRepository.FindAccountIdByWarehouseIdAsync(command.WarehouseId);
        
        // Gets the warehouse to update.
        var warehouseToUpdate = await warehouseRepository.FindByIdAsync(command.WarehouseId)
                                ?? throw new WarehouseFailedUpdateException($"Warehouse with ID {command.WarehouseId} does not exist.");
        
        // Verifies that the warehouse does not already exist with the same name.
        if (await warehouseRepository.ExistsByNameIgnoreCaseAndAccountIdAndWarehouseIdIsNotAsync(
                command.Name, new AccountId(accountId), command.WarehouseId))
        {
            throw new ArgumentException($"Warehouse with name {command.Name} already exists.");
        }
        
        // Verifies that the warehouse does not already exist with the same address.
        if (await warehouseRepository.ExistsByStreetAndCityAndPostalCodeIgnoreCaseAndAccountIdAndWarehouseIdIsNotAsync(
                command.NewAddress.Street, 
                command.NewAddress.City, 
                command.NewAddress.PostalCode, 
                new AccountId(accountId), 
                command.WarehouseId))
        {
            throw new WarehouseFailedUpdateException(
                $"Warehouse with address " +
                $"{command.NewAddress.Street}, " +
                $"{command.NewAddress.City}, " +
                $"{command.NewAddress.PostalCode} already exists.");
        }
        
        // Gets the image url of the warehouse to update.
        var currentImageUrl = await warehouseRepository.FindImageUrlByWarehouseIdAsync(command.WarehouseId);
        
        // Replaces the image url if the new image url is not empty.
        var newImageUrl = currentImageUrl;
        
        // Verifies that the image url is not empty.
        if (command.Image != null)
        {
            inventoryImageService.DeleteImage(currentImageUrl);
            newImageUrl = inventoryImageService.UploadImage(command.Image);
        }
        
        // Updates the warehouse with the new details.
        warehouseToUpdate.UpdateWarehouse(command, new ImageUrl(newImageUrl));

        // Tries to update the warehouse in the repository.
        try
        {
            await warehouseRepository.UpdateAsync(warehouseToUpdate);
            return warehouseToUpdate;
        }
        // If the warehouse could not be updated, throws an exception.
        catch (WarehouseFailedUpdateException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    ///     This method handles the deletion of a warehouse based on the provided command.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details required to delete a warehouse.
    /// </param>
    public async Task Handle(DeleteWarehouseCommand command)
    {
        // Verifies that the warehouse exists.
        var warehouseToDelete = await warehouseRepository.FindByIdAsync(command.WarehouseId)
                                ?? throw new WarehouseFailedDeletionException($"Warehouse with ID {command.WarehouseId} does not exist.");
        
        // Gets the image url of the warehouse to delete.
        var imageUrl = await warehouseRepository.FindImageUrlByWarehouseIdAsync(command.WarehouseId);
        
        // Deletes the image from the cloudinary service.
        inventoryImageService.DeleteImage(imageUrl);
        
        // Deletes the warehouse from the repository.
        await warehouseRepository.DeleteAsync(warehouseToDelete.Id.ToString());
    }
}