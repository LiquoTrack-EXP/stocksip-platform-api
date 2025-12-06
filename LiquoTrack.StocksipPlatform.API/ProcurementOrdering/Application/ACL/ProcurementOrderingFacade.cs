using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.ACL;

public class ProcurementOrderingFacade : IProcurementOrderingFacade
{
    private readonly IPurchaseOrderRepository _orderRepository;

    public ProcurementOrderingFacade(IPurchaseOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }
    
    public async Task<PurchaseOrderResource> GetPurchaseOrderResourceAsync(string purchaseOrderId)
    {
        var order = await _orderRepository.GetByIdAsync(new PurchaseOrderId(purchaseOrderId))
                    ?? throw new InvalidOperationException($"Purchase order {purchaseOrderId} not found.");

        return new PurchaseOrderResource(
            id: order.PurchaseOrderId.GetId,
            orderCode: order.OrderCode,
            items: order.Items.Select(i => new PurchaseOrderItemResource(
                productId: i.ProductId.GetId,
                productName: i.ProductName,
                quantity: i.Quantity,
                unitPrice: i.UnitPrice,
                subTotal: i.CalculateSubTotal()
            )).ToList(),
            status: order.Status.ToString(),
            catalogIdBuyFrom: order.CatalogIdBuyFrom.GetId(),
            generationDate: order.GenerationDate,
            confirmationDate: order.ConfirmationDate,
            buyer: order.Buyer.GetId,
            isOrderSent: order.IsOrderSent,
            total: order.Items.Sum(i => i.CalculateSubTotal())
        );
    }
}
