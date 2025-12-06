using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using ESalesOrderStatuses = LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects.ESalesOrderStatuses;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;

public record GenerateSalesOrderCommand(
    string orderCode, 
    PurchaseOrderId purchaseOrderId, 
    ICollection<SalesOrderItem> items, 
    ESalesOrderStatuses status, 
    CatalogId catalogToBuyFrom, 
    DateTime receiptDate, 
    DateTime completitionDate, 
    AccountId accountId
);