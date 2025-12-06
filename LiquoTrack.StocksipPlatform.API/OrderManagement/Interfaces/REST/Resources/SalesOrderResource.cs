namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources
{
    public record SalesOrderResource(
        string Id,
        string OrderCode,
        string PurchaseOrderId,
        IEnumerable<SalesOrderItemResource> Items,
        string Status,
        string CatalogToBuyFrom,
        DateTime ReceiptDate,
        DateTime CompletitionDate,
        string Buyer,
        DeliveryProposalResource? DeliveryProposal,
        string? SupplierId
    );
}
