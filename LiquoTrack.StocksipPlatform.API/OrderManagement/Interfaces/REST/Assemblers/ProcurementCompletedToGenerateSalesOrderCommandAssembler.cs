using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using ESalesOrderStatuses = LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects.ESalesOrderStatuses;
using SharedPurchaseOrderId = LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.PurchaseOrderId;
using SharedCatalogId = LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.CatalogId;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Assemblers;

public static class ProcurementCompletedToGenerateSalesOrderCommandAssembler
{
    public static GenerateSalesOrderCommand ToCommand(ProcurementOrderCompletedResource resource)
    {
        var items = new List<SalesOrderItem>();
        foreach (var it in resource.Items)
        {
            var unit = new Money(it.UnitPrice, new Currency(EValidCurrencyCodes.USD.ToString()));
            var soi = new SalesOrderItem(new ProductId(it.ProductId), unit, it.Quantity, it.ProductName)
            {
                InventoryId = new InventoryId(Guid.NewGuid().ToString())
            };
            items.Add(soi);
        }

        return new GenerateSalesOrderCommand(
            orderCode: resource.OrderCode,
            purchaseOrderId: new SharedPurchaseOrderId(resource.PurchaseOrderId),
            items: items,
            status: ESalesOrderStatuses.Received,
            catalogToBuyFrom: new SharedCatalogId(resource.CatalogIdBuyFrom),
            receiptDate: resource.ReceiptDate ?? DateTime.UtcNow,
            completitionDate: resource.CompletitionDate ?? DateTime.UtcNow.AddDays(7),
            accountId: new AccountId(resource.BuyerAccountId)
        );
    }
}
