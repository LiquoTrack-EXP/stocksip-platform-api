using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Assemblers;

public static class CreateSalesOrderCommandFromResourceAssembler
{
    public static GenerateSalesOrderCommand ToCommandFromResource(CreateSalesOrderResource resource, string accountId)
    {
        ArgumentNullException.ThrowIfNull(resource);

        if (resource.Items is null)
        {
            throw new ArgumentException("Sales order items cannot be null.", nameof(resource));
        }

        if (!Enum.TryParse(resource.Status, true, out ESalesOrderStatuses status))
        {
            throw new ArgumentException("Invalid sales order status value.", nameof(resource.Status));
        }

        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new ArgumentException("An account identifier must be provided via authentication.", nameof(accountId));
        }

        var items = resource.Items.Select(item =>
        {
            var salesOrderItem = new SalesOrderItem(
                new ProductId(item.ProductId),
                new Money(item.UnitPrice, new Currency(item.Currency)),
                item.QuantityToSell,
                item.ProductName);

            if (!string.IsNullOrWhiteSpace(item.InventoryId))
            {
                salesOrderItem.InventoryId = new InventoryId(item.InventoryId);
            }

            return salesOrderItem;
        }).ToList();

        var accountIdValue = accountId;
        if (string.IsNullOrWhiteSpace(accountIdValue))
        {
            throw new ArgumentException("Account ID is required for creating an order");
        }

        return new GenerateSalesOrderCommand(
            resource.OrderCode,
            new Shared.Domain.Model.ValueObjects.PurchaseOrderId(resource.PurchaseOrderId),
            items,
            status,
            new Shared.Domain.Model.ValueObjects.CatalogId(resource.CatalogToBuyFrom),
            resource.ReceiptDate,
            resource.CompletitionDate,
            AccountId.Create(accountIdValue.Trim())
        );
    }
}
