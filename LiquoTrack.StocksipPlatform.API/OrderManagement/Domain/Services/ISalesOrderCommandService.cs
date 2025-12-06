using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;

public interface ISalesOrderCommandService
{
    /// <summary>
    /// Handles the creation of a new sales order
    /// </summary>
    Task<SalesOrder> Handle(GenerateSalesOrderCommand command);
    
    /// <summary>
    /// Generate a purchase order (manual or automatic)
    /// </summary>
    /// <param name="request">The purchase order request</param>
    /// <param name="accountId">The account ID of the buyer</param>
    /// <returns>The created sales order</returns>
    Task<SalesOrder> GeneratePurchaseOrder(GeneratePurchaseOrderRequest request, string accountId);
    
    /// <summary>
    /// Updates the status of an existing order
    /// </summary>
    /// <param name="command">The update status command</param>
    /// <returns>The updated sales order</returns>
    Task<SalesOrder> Handle(UpdateOrderStatusCommand command);

    /// <summary>
    /// Supplier proposes a delivery schedule for an order
    /// </summary>
    Task<SalesOrder> Handle(ProposeDeliveryScheduleCommand command);

    /// <summary>
    /// LiquorStoreOwner responds to a delivery proposal (accept/reject)
    /// </summary>
    Task<SalesOrder> Handle(RespondDeliveryProposalCommand command);
    
    
    Task<SalesOrder> CreateFromPurchaseOrderAsync(string purchaseOrderId);
    
    Task<SalesOrder> ConfirmOrderAsync(string orderId);
    Task<SalesOrder> ReceiveOrderAsync(string orderId);
    Task<SalesOrder> ShipOrderAsync(string orderId);
    Task<SalesOrder> CancelOrderAsync(string orderId);
}