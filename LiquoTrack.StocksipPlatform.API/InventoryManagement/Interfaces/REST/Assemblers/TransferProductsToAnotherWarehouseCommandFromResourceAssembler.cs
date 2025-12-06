using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Static assembler class to convert TransferProductsToAnotherWarehouseResource to TransferProductsToAnotherWarehouseCommand.
/// </summary>
public static class TransferProductsToAnotherWarehouseCommandFromResourceAssembler
{
    /// <summary>
    ///     Static method to convert TransferProductsToAnotherWarehouseResource to TransferProductsToAnotherWarehouseCommand.
    /// </summary>
    /// <param name="resource">
    ///     The TransferProductsToAnotherWarehouseResource to convert.
    /// </param>
    /// <param name="originWarehouseId">
    ///     The origin warehouse id as string.
    /// </param>
    /// <param name="productId">
    ///     The product id as string.
    /// </param>
    /// <returns>
    ///     A new instance of TransferProductsToAnotherWarehouseCommand.
    /// </returns>
    public static TransferProductsToAnotherWarehouseCommand ToCommandFromResource(
        TransferProductsToAnotherWarehouseResource resource, string originWarehouseId, string productId)
    {
        // Converts the resource's id strings to ObjectId instances'
        var targetProductId = new ObjectId(productId);
        var targetOriginWarehouseId = new ObjectId(originWarehouseId);
        var targetDestinationWarehouseId = new ObjectId(resource.DestinationWarehouseId);
        
        // Validates if the resource has an expiration date.
        if (resource.ExpirationDate != null)
        {
            // If it does, creates a new TransferProductsToAnotherWarehouseCommand with the expiration date.
            return new TransferProductsToAnotherWarehouseCommand(
                targetProductId,
                targetOriginWarehouseId,
                resource.ExpirationDate.Value,
                resource.QuantityToTransfer,
                targetDestinationWarehouseId);
        }
        
        // If the resource does not have an expiration date, creates a new TransferProductsToAnotherWarehouseCommand without an expiration date.
        return new TransferProductsToAnotherWarehouseCommand(
            targetProductId,
            targetOriginWarehouseId,
            null,
            resource.QuantityToTransfer,
            targetDestinationWarehouseId);
    }
}