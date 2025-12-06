using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.ACL;

/// <summary>
/// ACL interface for exposing Product context operations to other bounded contexts.
/// </summary>
public interface IProductContextFacade
{
    /// <summary>
    /// Retrieves essential product information by its ID for use in other contexts.
    /// </summary>
    /// <param name="productId">The ID of the product to fetch.</param>
    /// <returns>A resource containing product information, or null if not found.</returns>
    Task<ProductDetailsResource?> GetProductDetailsByIdAsync(string productId);
    
    /// <summary>
    /// Retrieves the available stock of a product in a specific warehouse.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <param name="warehouseId">The ID of the warehouse.</param>
    /// <returns>The available stock quantity, or null if inventory not found.</returns>
    Task<int?> GetProductStockInWarehouseAsync(string productId, string warehouseId);
    
    /// <summary>
    /// Retrieves complete inventory details for a product in a warehouse.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <param name="warehouseId">The ID of the warehouse.</param>
    /// <returns>A resource containing inventory information, or null if not found.</returns>
    Task<InventoryDetailsResource?> GetInventoryDetailsAsync(string productId, string warehouseId);
}