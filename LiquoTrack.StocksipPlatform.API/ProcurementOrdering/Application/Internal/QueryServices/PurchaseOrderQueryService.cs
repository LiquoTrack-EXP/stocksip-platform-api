using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.Internal.QueryServices;

/// <summary>
/// Service implementation for handling purchase order queries.
/// </summary>
public class PurchaseOrderQueryService(IPurchaseOrderRepository orderRepository) : IPurchaseOrderQueryService
{
    /// <summary>
    /// Handles the GetPurchaseOrderByIdQuery to retrieve a purchase order by its identifier.
    /// </summary>
    /// <param name="query">The query containing the order identifier.</param>
    /// <returns>The purchase order if found, otherwise null.</returns>
    public async Task<PurchaseOrder?> Handle(GetPurchaseOrderByIdQuery query)
    {
        var orderId = new PurchaseOrderId(query.orderId);
        return await orderRepository.GetByIdAsync(orderId);
    }

    /// <summary>
    /// Handles the GetAllPurchaseOrdersQuery to retrieve all purchase orders.
    /// </summary>
    /// <param name="query">The query to get all orders.</param>
    /// <returns>A collection of purchase orders.</returns>
    public async Task<IEnumerable<PurchaseOrder>> Handle(GetAllPurchaseOrdersQuery query)
    {
        return await orderRepository.GetAllAsync();
    }

    /// <summary>
    /// Handles the GetOrdersByBuyerQuery to retrieve purchase orders for a specific buyer.
    /// </summary>
    /// <param name="query">The query containing the buyer identifier.</param>
    /// <returns>A collection of purchase orders for the buyer.</returns>
    public async Task<IEnumerable<PurchaseOrder>> Handle(GetOrdersByBuyerQuery query)
    {
        var buyerId = new AccountId(query.buyer);
        return await orderRepository.FindByBuyerAsync(buyerId);
    }

    /// <summary>
    /// Handles the GetOrdersByCatalogQuery to retrieve purchase orders from a specific catalog.
    /// </summary>
    /// <param name="query">The query containing the catalog identifier.</param>
    /// <returns>A collection of purchase orders from the catalog.</returns>
    public async Task<IEnumerable<PurchaseOrder>> Handle(GetOrdersByCatalogQuery query)
    {
        var catalogId = new CatalogId(query.catalogId);
        return await orderRepository.FindByCatalogAsync(catalogId);
    }
}