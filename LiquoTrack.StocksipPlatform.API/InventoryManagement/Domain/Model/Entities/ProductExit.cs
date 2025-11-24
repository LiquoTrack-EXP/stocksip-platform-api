using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;

/// <summary>
///     Class for recording a product exit of an inventory.
/// </summary>
/// <param name="productId">
///     The ID of the product that was exited.
/// </param>
/// <param name="productName">
///     The name of the product that was exited.
/// </param>
/// <param name="warehouseId">
///     The ID of the warehouse from which the product was exited.
/// </param>
/// <param name="warehouseName">
///     The name of the warehouse from which the product was exited.
/// </param>
/// <param name="expirationDate">
///     The expiration date of the product that was exited if applicable.
/// </param>    
/// <param name="exitType">
///     The reason for the product exit.
/// </param>
/// <param name="outputQuantity">
///     The quantity of the product that was exited.
/// </param>
/// <param name="previousQuantity">
///     The previous quantity of the product in the warehouse.
/// </param>
public class ProductExit(
    string productId, 
    string productName, 
    string warehouseId,
    string warehouseName, 
    EProductExitReasons exitType,
    int outputQuantity,
    int previousQuantity,
    string? expirationDate = ""
) : Entity 
{
    /// <summary>
    ///     The ID of the product that was exited.
    /// </summary>
    public string ProductId { get; private set; } = productId;
    
    /// <summary>
    ///     The name of the product that was exited.
    /// </summary>
    public string ProductName { get; private set; } = productName;

    /// <summary>
    ///     The expiration date of the product that was exited if applicable.
    /// </summary>
    public string? ExpirationDate { get; private set; } = expirationDate ?? string.Empty;
    
    /// <summary>
    ///     The ID of the warehouse from which the product was exited.
    /// </summary>
    public string WarehouseId { get; private set; } = warehouseId;
    
    /// <summary>
    ///     The name of the warehouse from which the product was exited.
    /// </summary>
    public string WarehouseName { get; private set; } = warehouseName;
    
    /// <summary>
    ///     The reason for the product exit.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public EProductExitReasons ExitType { get; private set; } = exitType;

    /// <summary>
    ///     The quantity of the product that was exited.
    /// </summary>
    public int OutputQuantity { get; private set; } = outputQuantity;
    
    /// <summary>
    ///     The previous quantity of the product in the warehouse.
    /// </summary>
    public int PreviousQuantity { get; private set; } = previousQuantity;
}