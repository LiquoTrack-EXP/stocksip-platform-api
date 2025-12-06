using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;

/// <summary>
/// Interface for purchase order query service.
/// </summary>
public interface IPurchaseOrderQueryService
{
    /// <summary>
    /// Handles the GetPurchaseOrderByIdQuery.
    /// </summary>
    /// <param name="query">The query to get a purchase order by id.</param>
    /// <returns>The purchase order if found, otherwise null.</returns>
    Task<PurchaseOrder?> Handle(GetPurchaseOrderByIdQuery query);

    /// <summary>
    /// Handles the GetAllPurchaseOrdersQuery.
    /// </summary>
    /// <param name="query">The query to get all purchase orders.</param>
    /// <returns>A collection of purchase orders.</returns>
    Task<IEnumerable<PurchaseOrder>> Handle(GetAllPurchaseOrdersQuery query);

    /// <summary>
    /// Handles the GetOrdersByBuyerQuery.
    /// </summary>
    /// <param name="query">The query to get orders by buyer.</param>
    /// <returns>A collection of purchase orders for the buyer.</returns>
    Task<IEnumerable<PurchaseOrder>> Handle(GetOrdersByBuyerQuery query);

    /// <summary>
    /// Handles the GetOrdersByCatalogQuery.
    /// </summary>
    /// <param name="query">The query to get orders by catalog.</param>
    /// <returns>A collection of purchase orders from the catalog.</returns>
    Task<IEnumerable<PurchaseOrder>> Handle(GetOrdersByCatalogQuery query);
}