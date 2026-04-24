using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Static assembler class to convert ProductTransfer entity to ProductTransferResource.
/// </summary>
public static class ProductTransferResourceFromEntityAssembler
{
    /// <summary>
    ///     Static method to convert ProductTransfer entity to ProductTransferResource.
    /// </summary>
    /// <param name="entity">
    ///     The ProductTransfer entity to convert.
    /// </param>
    /// <returns>
    ///     A ProductTransferResource representation of the ProductTransfer entity.
    /// </returns>
    public static ProductTransferResource ToResourceFromEntity(ProductTransfer entity)
    {
        return new ProductTransferResource(
            entity.Id.ToString(),
            entity.ProductId,
            entity.ProductName,
            entity.OriginWarehouseId,
            entity.OriginWarehouseName,
            entity.DestinationWarehouseId,
            entity.DestinationWarehouseName,
            entity.TransferredQuantity,
            entity.OriginWarehouseRemainingStock,
            entity.DestinationWarehouseResultingStock,
            entity.CreatedAt,
            entity.ExpirationDate ?? string.Empty
        );
    }
}