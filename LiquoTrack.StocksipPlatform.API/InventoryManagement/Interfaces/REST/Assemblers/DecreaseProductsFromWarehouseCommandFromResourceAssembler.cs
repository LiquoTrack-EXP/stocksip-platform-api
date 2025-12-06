using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Static assembler class to convert DecreaseProductsFromWarehouseResource to DecreaseProductsFromWarehouseCommand.
/// </summary>
public static class DecreaseProductsFromWarehouseCommandFromResourceAssembler
{
    /// <summary>
    ///     Static method to convert DecreaseProductsFromWarehouseResource to DecreaseProductsFromWarehouseCommand.
    /// </summary>
    /// <param name="resource">
    ///     The DecreaseProductsFromWarehouseResource to convert.
    /// </param>
    /// <param name="productId">
    ///     The product id as string.
    /// </param>
    /// <param name="warehouseId">
    ///     The warehouse id as string.
    /// </param>
    /// <returns>
    ///     A new instance of DecreaseProductsFromWarehouseCommand.   
    /// </returns>
    public static DecreaseProductsFromWarehouseCommand ToCommandFromResource(
        DecreaseProductsFromWarehouseResource resource, string productId, string warehouseId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        if (!resource.ExpirationDate.HasValue)
            throw new ArgumentNullException(nameof(resource.ExpirationDate), "ExpirationDate is required for this command.");
        
        var dateOnly = DateOnly.FromDateTime(resource.ExpirationDate.Value);

        var exitType = Enum.Parse<EProductExitReasons>(resource.ExitType);
        
        return new DecreaseProductsFromWarehouseCommand(
                new ObjectId(productId),
                new ObjectId(warehouseId),
                new ProductExpirationDate(dateOnly),
                resource.QuantityToDecrease,
                exitType
            );
    }

    /// <summary>
    ///     Static method to convert DecreaseProductsFromWarehouseResource to DecreaseProductsFromWarehouseWithoutExpirationDateCommand.
    /// </summary>
    /// <param name="resource">
    ///     The DecreaseProductsFromWarehouseResource to convert. 
    /// </param>
    /// <param name="productId">
    ///     The product id as string.
    /// </param>
    /// <param name="warehouseId">
    ///     The warehouse id as string. 
    /// </param>
    /// <returns></returns>
    public static DecreaseProductsFromWarehouseWithoutExpirationDateCommand ToCommandFromResourceWithoutExpirationDate(
        DecreaseProductsFromWarehouseResource resource, string productId, string warehouseId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        
        var exitType = Enum.Parse<EProductExitReasons>(resource.ExitType);
        
        return new DecreaseProductsFromWarehouseWithoutExpirationDateCommand(
            new ObjectId(productId),
            new ObjectId(warehouseId),
            resource.QuantityToDecrease,
            exitType
        );
    }
}