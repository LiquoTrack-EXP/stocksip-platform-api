using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;

/// <summary>
///     Class for recording a product transfer of an inventory.
/// </summary>
/// <param name="productId">
///     The ID of the product that was transferred.
/// </param>
/// <param name="productName">
///     The name of the product that was transferred.
/// </param>
/// <param name="originWarehouseId">
///     The ID of the warehouse from which the product was transferred.
/// </param>
/// <param name="originWarehouseName">
///     The name of the original warehouse.
/// </param>
/// <param name="destinationWarehouseId">
///     The ID of the destination warehouse.   
/// </param>
/// <param name="destinationWarehouseName">
///     the name of the destination warehouse.
/// </param>
/// <param name="transferredStock">
///     The quantity of the product that was transferred.
/// </param>
/// <param name="originWarehouseRemainingStock">
///     The remaining stock of the product in the origin warehouse.
/// </param>
/// <param name="destinationWarehouseResultingStock">
///     The new quantity of the product in the destination warehouse.
/// </param>
/// <param name="expirationDate">
///     The expiration date of the product that was transferred if applicable.
/// </param>
public class ProductTransfer(
    string productId, 
    string productName, 
    string originWarehouseId, 
    string originWarehouseName,
    string destinationWarehouseId,
    string destinationWarehouseName,
    int transferredStock, 
    int originWarehouseRemainingStock,
    int destinationWarehouseResultingStock,
    string? expirationDate
) : Entity
{
    public string ProductId { get; private set; } = productId;
    
    /// <summary>
    ///     The name of the product that was exited.
    /// </summary>
    public string ProductName { get; private set; } = productName;

    /// <summary>
    ///     The expiration date of the product that was transferred if applicable.
    /// </summary>
    public string? ExpirationDate { get; private set; } = expirationDate ?? string.Empty;
    
    /// <summary>
    ///     The ID of the warehouse from which the product was transferred.
    /// </summary>
    public string OriginWarehouseId { get; private set; } = originWarehouseId;
    
    /// <summary>
    ///     The name of the original warehouse.
    /// </summary>
    public string OriginWarehouseName { get; private set; } = originWarehouseName;
    
    /// <summary>
    ///     Id of the destination warehouse.     
    /// </summary>
    public string DestinationWarehouseId { get; private set; } = destinationWarehouseId;
    
    /// <summary>
    ///     Name of the destination warehouse.
    /// </summary>
    public string DestinationWarehouseName { get; private set; } = destinationWarehouseName;

    /// <summary>
    ///     The quantity of the product that was transferred.
    /// </summary>
    public int TransferredQuantity { get; private set; } = transferredStock;
    
    /// <summary>
    ///     The remaining stock of the product in the origin warehouse.
    /// </summary>
    public int OriginWarehouseRemainingStock { get; private set; } = originWarehouseRemainingStock;

    /// <summary>
    ///     The new quantity of the product in the destination warehouse.
    /// </summary>
    public int DestinationWarehouseResultingStock { get; private set; } = destinationWarehouseResultingStock;
}