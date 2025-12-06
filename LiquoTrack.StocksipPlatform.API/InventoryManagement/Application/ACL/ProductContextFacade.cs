using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.ACL;

/// <summary>
/// Facade for handling product-related operations, exposed to other bounded contexts.
/// </summary>
public class ProductContextFacade : IProductContextFacade
{
    private readonly IProductQueryService _productQueryService;
    private readonly IInventoryQueryService _inventoryQueryService;
    private readonly ILogger<ProductContextFacade> _logger;

    public ProductContextFacade(
        IProductQueryService productQueryService,
        IInventoryQueryService inventoryQueryService,
        ILogger<ProductContextFacade> logger)
    {
        _productQueryService = productQueryService ?? throw new ArgumentNullException(nameof(productQueryService));
        _inventoryQueryService = inventoryQueryService ?? throw new ArgumentNullException(nameof(inventoryQueryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves essential product information by its ID for use in other contexts.
    /// </summary>
    /// <param name="productId">The ID of the product to fetch.</param>
    /// <returns>A resource containing product information, or null if not found.</returns>
    public async Task<ProductDetailsResource?> GetProductDetailsByIdAsync(string productId)
    {
        if (!ObjectId.TryParse(productId, out var objectId))
            throw new ArgumentException("Invalid product ID format.", nameof(productId));

        try
        {
            _logger.LogInformation("Fetching product details for product ID: {ProductId}", productId);

            var query = new GetProductByIdQuery(objectId);
            var product = await _productQueryService.Handle(query);

            if (product == null)
            {
                _logger.LogWarning("No product found with ID: {ProductId}", productId);
                return null;
            }

            var resource = new ProductDetailsResource
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Brand = product.Brand,
                Type = product.Type.ToString(),
                UnitPrice = product.UnitPrice.GetAmount(),
                Currency = product.UnitPrice.GetCurrencyCode(),
                ImageUrl = product.ImageUrl.GetValue()
            };

            _logger.LogInformation("Successfully retrieved product details for ID: {ProductId}", productId);
            return resource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product details for ID: {ProductId}", productId);
            throw new InvalidOperationException("An error occurred while fetching product details.", ex);
        }
    }

    /// <summary>
    /// Retrieves the available stock of a product in a specific warehouse.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <param name="warehouseId">The ID of the warehouse.</param>
    /// <returns>The available stock quantity, or null if inventory not found.</returns>
    public async Task<int?> GetProductStockInWarehouseAsync(string productId, string warehouseId)
    {
        if (!ObjectId.TryParse(productId, out var productObjectId))
            throw new ArgumentException("Invalid product ID format.", nameof(productId));

        if (!ObjectId.TryParse(warehouseId, out var warehouseObjectId))
            throw new ArgumentException("Invalid warehouse ID format.", nameof(warehouseId));

        try
        {
            _logger.LogInformation("Fetching stock for product {ProductId} in warehouse {WarehouseId}", 
                productId, warehouseId);

            var query = new GetInventoryByProductIdAndWarehouseIdQuery(productObjectId, warehouseObjectId);
            var inventory = await _inventoryQueryService.Handle(query);

            if (inventory == null)
            {
                _logger.LogWarning("No inventory found for product {ProductId} in warehouse {WarehouseId}", 
                    productId, warehouseId);
                return null;
            }

            _logger.LogInformation("Stock retrieved: {Quantity} units for product {ProductId} in warehouse {WarehouseId}", 
                inventory.Quantity.GetValue, productId, warehouseId);
            
            return inventory.Quantity.GetValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock for product {ProductId} in warehouse {WarehouseId}", 
                productId, warehouseId);
            throw new InvalidOperationException("An error occurred while fetching product stock.", ex);
        }
    }

    /// <summary>
    /// Retrieves complete inventory details for a product in a warehouse.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <param name="warehouseId">The ID of the warehouse.</param>
    /// <returns>A resource containing inventory information, or null if not found.</returns>
    public async Task<InventoryDetailsResource?> GetInventoryDetailsAsync(string productId, string warehouseId)
    {
        if (!ObjectId.TryParse(productId, out var productObjectId))
            throw new ArgumentException("Invalid product ID format.", nameof(productId));

        if (!ObjectId.TryParse(warehouseId, out var warehouseObjectId))
            throw new ArgumentException("Invalid warehouse ID format.", nameof(warehouseId));

        try
        {
            _logger.LogInformation("Fetching inventory details for product {ProductId} in warehouse {WarehouseId}", 
                productId, warehouseId);

            var query = new GetInventoryByProductIdAndWarehouseIdQuery(productObjectId, warehouseObjectId);
            var inventory = await _inventoryQueryService.Handle(query);

            if (inventory == null)
            {
                _logger.LogWarning("No inventory found for product {ProductId} in warehouse {WarehouseId}", 
                    productId, warehouseId);
                return null;
            }

            var resource = new InventoryDetailsResource
            {
                Id = inventory.Id.ToString(),
                ProductId = inventory.ProductId.ToString(),
                WarehouseId = inventory.WarehouseId.ToString(),
                Quantity = inventory.Quantity.GetValue,
                CurrentState = inventory.CurrentState.ToString(),
                ExpirationDate = inventory.ExpirationDate?.GetValue()
            };

            _logger.LogInformation("Successfully retrieved inventory details for product {ProductId} in warehouse {WarehouseId}", 
                productId, warehouseId);
            
            return resource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching inventory details for product {ProductId} in warehouse {WarehouseId}", 
                productId, warehouseId);
            throw new InvalidOperationException("An error occurred while fetching inventory details.", ex);
        }
    }
}