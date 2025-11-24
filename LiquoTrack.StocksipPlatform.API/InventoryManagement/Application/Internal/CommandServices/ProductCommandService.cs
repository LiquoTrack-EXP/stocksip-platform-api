using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.OutboundServices.FileStorage;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Exceptions;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;

/// <summary>
///     Command service implementation for handling product-related commands.
/// </summary>
/// <param name="productRepository">
///     The repository for handling the Products in the database.
/// </param>
/// <param name="inventoryImageService">
///     The service for handling image operations.
/// </param>
/// <param name="inventoryRepository">
///     The repository for handling the Inventories in the database.
/// </param>
public class ProductCommandService(
        IProductRepository productRepository,
        IInventoryImageService inventoryImageService,
        IInventoryRepository inventoryRepository,
        IPaymentAndSubscriptionsFacade paymentAndSubscriptionsFacade
    ) : IProductCommandService
{
    /// <summary>
    ///     Method to handle the registration of a new product.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for registering a new product.
    /// </param>
    /// <returns>
    ///     The newly registered product.
    ///     Or null if the product could not be registered.
    /// </returns>
    public async Task<Product?> Handle(RegisterProductCommand command)
    {
        // Verifies that the product name is unique.
        if (await productRepository.ExistsByNameAndAccountIdAsync(new ProductName(command.Name), command.AccountId))
        {
            throw new ProductFailedCreationException($"This {command.Name} is taken by another product. Cannot create a new product with the same name.");
        }
        
        // Verifies that the account has not reached the maximum number of warehouses.
        var currentProductsCount = await productRepository.CountByAccountIdAsync(command.AccountId);
        
        var maxAllowedProducts = await paymentAndSubscriptionsFacade
            .GetPlanWarehouseLimitByAccountId(command.AccountId.GetId);
        
        if (maxAllowedProducts != 0 && currentProductsCount >= maxAllowedProducts)
            throw new ProductFailedCreationException($"The account {command.AccountId.GetId} has reached the maximum number of products for the current plan.");
        
        // Gets the image url of the warehouse.
        string imageUrl = command.Image != null
            ? inventoryImageService.UploadImage(command.Image)
            : "https://res.cloudinary.com/deuy1pr9e/image/upload/v1759709979/Default-product_kt9bxf.png";

        // Creates the product with the given details.
        var product = new Product(command, imageUrl);

        // Tries to add the product to the repository.
        try
        {
            await productRepository.AddAsync(product);
            return product;
        }
        // If the product could not be added, throws an exception.
        catch (ProductFailedCreationException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    ///     Method to handle the update of product information.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for updating the product information.
    /// </param>
    /// <returns>
    ///     The updated product.
    ///     Or null if the product could not be updated.
    /// </returns>
    public async Task<Product?> Handle(UpdateProductInformationCommand command)
    {
        // Verifies that the product exists.
        var productToUpdate = await productRepository.FindByIdAsync(command.ProductId.ToString())
                              ?? throw new ProductFailedUpdateException($"Could not find the product to update with identifier ${command.ProductId.ToString()}.");
        
        var currentImageUrl = await productRepository.FindImageUrlByProductIdAsync(command.ProductId);
        var imageUrl = currentImageUrl;

        if (command.Image != null)
        {
            inventoryImageService.DeleteImage(currentImageUrl);
            imageUrl = inventoryImageService.UploadImage(command.Image);
        }
        
        // Updates the product with the given details.
        productToUpdate.UpdateInformation(command, imageUrl);

        // Tries to update the product in the repository.
        try
        {
            await productRepository.UpdateAsync(command.ProductId.ToString(), productToUpdate);
        }
        // If the product could not be updated, throws an exception.
        catch (ProductFailedUpdateException e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        // Returns the updated product.
        return productToUpdate;
    }

    /// <summary>
    ///     Method to handle the update of the minimum stock related to a product.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for updating the minimum stock.
    /// </param>
    /// <returns>
    ///     The updated product.
    ///     Or null if the product could not be updated.   
    /// </returns>
    public async Task<Product?> Handle(UpdateProductMinimumStockCommand command)
    {
        // Verifies that the product exists.
        var productToUpdate = await productRepository.FindByIdAsync(command.ProductId.ToString())
                              ?? throw new ProductFailedUpdateException($"Could not find the product to update with identifier ${command.ProductId.ToString()}.");
        
        // Updates the product with the given details.
        productToUpdate.UpdateMinimumStock(command);

        // Tries to update the product minimum stock in the repository.
        try
        {
            await productRepository.UpdateAsync(command.ProductId.ToString(), productToUpdate);
        }
        // If the product could not be updated, throws an exception.
        catch (ProductFailedUpdateException e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        // Returns the updated product.
        return productToUpdate;
    }

    /// <summary>
    ///     Method to delete a product.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for deleting a product.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    ///     No return value.
    /// </returns>
    public async Task Handle(DeleteProductCommand command)
    {
        // Verifies that the product exists.
        var product = await productRepository.FindByIdAsync(command.ProductId.ToString());
        if (product == null)
            throw new ProductFailedDeletionException($"Could not find the product to delete with identifier {command.ProductId.ToString()}.");

        
        if (product.TotalStockInStore != 0)
            throw new ProductFailedDeletionException($"Cannot delete product with identifier {command.ProductId.ToString()} because it still has stock in the warehouse.");
            
        var imageUrl = await productRepository.FindImageUrlByProductIdAsync(command.ProductId);
        
        // Tries to delete the product and its inventories from the repository.
        try
        {
            var inventories = await inventoryRepository.FindByProductIdAsync(command.ProductId);
            foreach (var inventory in inventories)
            {
                await inventoryRepository.DeleteAsync(inventory.Id.ToString());
            }
            inventoryImageService.DeleteImage(imageUrl);
            await productRepository.DeleteAsync(command.ProductId.ToString());
        }
        // If the product could not be deleted, throws an exception.
        catch (ProductFailedDeletionException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}