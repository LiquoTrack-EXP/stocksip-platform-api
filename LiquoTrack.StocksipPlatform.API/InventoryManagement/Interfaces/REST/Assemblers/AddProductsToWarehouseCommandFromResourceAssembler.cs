using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Static assembler class to convert AddProductsToWarehouseResource to AddProductsToWarehouseCommand.
/// </summary>
public static class AddProductsToWarehouseCommandFromResourceAssembler
{
    /// <summary>
    ///     Static method to convert AddProductsToWarehouseResource to AddProductsToWarehouseCommand. 
    /// </summary>
    /// <param name="resource">
    ///     An AddProductsToWarehouseResource to convert.
    /// </param>
    /// <param name="productId">
    ///     The product id as string.
    /// </param>
    /// <param name="warehouseId">
    ///     The warehouse id as string.
    /// </param>
    /// <returns>
    ///     The AddProductsToWarehouseCommand representation of the AddProductsToWarehouseResource.
    /// </returns>
    public static AddProductsToWarehouseCommand ToCommandFromResource(
        AddProductsToWarehouseResource resource, string productId, string warehouseId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        if (!resource.ExpirationDate.HasValue)
            throw new ArgumentNullException(nameof(resource.ExpirationDate), "ExpirationDate is required for this command.");

        var dateOnly = DateOnly.FromDateTime(resource.ExpirationDate.Value);

        return new AddProductsToWarehouseCommand(
            new ObjectId(productId),
            new ObjectId(warehouseId),
            new ProductExpirationDate(dateOnly),
            resource.QuantityToAdd
        );
    }

    /// <summary>
    ///     Static method to convert AddProductsToWarehouseResource to AddProductsToWarehouseWithoutExpirationDateCommand.
    /// </summary>
    /// <param name="resource">
    ///     The AddProductsToWarehouseResource to convert.
    /// </param>
    /// <param name="productId">
    ///     The product id as string.
    /// </param>
    /// <param name="warehouseId">
    ///     The warehouse id as string.
    /// </param>
    /// <returns>
    ///     A new instance of AddProductsToWarehouseWithoutExpirationDateCommand.
    /// </returns>
    public static AddProductsToWarehouseWithoutExpirationDateCommand ToCommandFromResourceWithoutExpirationDate(
        AddProductsToWarehouseResource resource, string productId, string warehouseId)
    {
        ArgumentNullException.ThrowIfNull(resource);

        return new AddProductsToWarehouseWithoutExpirationDateCommand(
            new ObjectId(productId),
            new ObjectId(warehouseId),
            resource.QuantityToAdd
        );
    }
}