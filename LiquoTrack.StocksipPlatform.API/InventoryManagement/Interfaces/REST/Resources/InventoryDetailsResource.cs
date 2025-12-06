namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
/// Data Transfer Object representing inventory details exposed to other contexts.
/// </summary>
public class InventoryDetailsResource
{
    /// <summary>
    /// The inventory identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The product identifier.
    /// </summary>
    public string ProductId { get; set; } = string.Empty;
    
    /// <summary>
    /// The warehouse identifier.
    /// </summary>
    public string WarehouseId { get; set; } = string.Empty;
    
    /// <summary>
    /// The available stock quantity.
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// The current state of the product (WithStock, LowStock, OutOfStock).
    /// </summary>
    public string CurrentState { get; set; } = string.Empty;
    
    /// <summary>
    /// The expiration date of the product, if applicable.
    /// </summary>
    public DateOnly? ExpirationDate { get; set; }
}