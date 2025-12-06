using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;

public static class SalesOrderResourceFromEntityAssembler
{
    public static SalesOrderResource ToResourceFromEntity(SalesOrder salesOrder)
    {
        ArgumentNullException.ThrowIfNull(salesOrder);

        return new SalesOrderResource(
            salesOrder.Id.ToString(),
            salesOrder.OrderCode,
            salesOrder.PurchaseOrderId.GetId,
            salesOrder.Items.Select(item => new SalesOrderItemResource(
                item.ProductId.GetId,
                item.ProductName ?? string.Empty,
                item.UnitPrice.GetAmount(),
                item.UnitPrice.GetCurrencyCode(),
                item.InventoryId?.GetId,
                item.QuantityToSell
            )),
            salesOrder.Status.ToString(),
            salesOrder.CatalogToBuyFrom.GetId(),
            salesOrder.ReceiptDate,
            salesOrder.CompletitionDate,
            salesOrder.AccountId.GetId,
            salesOrder.DeliveryProposal == null
                ? null
                : new DeliveryProposalResource(
                    salesOrder.DeliveryProposal.ProposedDate,
                    salesOrder.DeliveryProposal.Notes,
                    salesOrder.DeliveryProposal.Status.ToString(),
                    salesOrder.DeliveryProposal.CreatedAt,
                    salesOrder.DeliveryProposal.RespondedAt
                ),
            salesOrder.SupplierId?.GetId
        );
    }
}