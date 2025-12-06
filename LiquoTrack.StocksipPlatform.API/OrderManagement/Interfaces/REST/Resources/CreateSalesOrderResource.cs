namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources
{
    public record CreateSalesOrderResource(
        string OrderCode,
        string PurchaseOrderId,
        IEnumerable<CreateSalesOrderItemResource> Items,
        string Status,
        string CatalogToBuyFrom,
        DateTime ReceiptDate,
        DateTime CompletitionDate
    );
}
