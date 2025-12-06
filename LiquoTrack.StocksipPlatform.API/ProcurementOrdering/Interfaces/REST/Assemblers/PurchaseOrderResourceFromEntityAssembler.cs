using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;

/// <summary>
/// Assembler to convert PurchaseOrder entity to PurchaseOrderResource.
/// </summary>
public static class PurchaseOrderResourceFromEntityAssembler
{
    /// <summary>
    /// Converts a PurchaseOrder entity to a PurchaseOrderResource.
    /// </summary>
    /// <param name="entity">The purchase order entity.</param>
    /// <returns>The purchase order resource.</returns>
    public static PurchaseOrderResource ToResourceFromEntity(PurchaseOrder entity)
    {
        return new PurchaseOrderResource(
            entity.Id.ToString(),
            entity.OrderCode,
            entity.Items.Select(item => new PurchaseOrderItemResource(
                item.ProductId.GetId,
                item.ProductName,
                item.UnitPrice,
                item.Quantity,
                item.CalculateSubTotal()
            )).ToList(),
            entity.Status.ToString(),
            entity.CatalogIdBuyFrom.GetId(),
            entity.GenerationDate,
            entity.ConfirmationDate,
            entity.Buyer.GetId,
            entity.IsOrderSent,
            entity.CalculateTotal().GetAmount()
        );
    }

    /// <summary>
    /// Converts a collection of PurchaseOrder entities to PurchaseOrderResources.
    /// </summary>
    /// <param name="entities">The collection of purchase order entities.</param>
    /// <returns>The collection of purchase order resources.</returns>
    public static IEnumerable<PurchaseOrderResource> ToResourceFromEntity(IEnumerable<PurchaseOrder> entities)
    {
        return entities.Select(ToResourceFromEntity);
    }
}