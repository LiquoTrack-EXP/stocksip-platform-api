using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;

/// <summary>
/// Repository interface for purchase order operations.
/// </summary>
public interface IPurchaseOrderRepository : IBaseRepository<PurchaseOrder>
{
    /// <summary>
    /// Gets a purchase order by its identifier.
    /// </summary>
    /// <param name="id">The purchase order identifier.</param>
    /// <returns>The purchase order if found, otherwise null.</returns>
    Task<PurchaseOrder?> GetByIdAsync(PurchaseOrderId id);

    /// <summary>
    /// Gets a purchase order by its order code.
    /// </summary>
    /// <param name="orderCode">The order code.</param>
    /// <returns>The purchase order if found, otherwise null.</returns>
    Task<PurchaseOrder?> GetByOrderCodeAsync(string orderCode);

    /// <summary>
    /// Gets all purchase orders for a specific buyer.
    /// </summary>
    /// <param name="buyer">The buyer account identifier.</param>
    /// <returns>A collection of purchase orders for the buyer.</returns>
    Task<IEnumerable<PurchaseOrder>> FindByBuyerAsync(AccountId buyer);

    /// <summary>
    /// Gets all purchase orders from a specific catalog.
    /// </summary>
    /// <param name="catalogId">The catalog identifier.</param>
    /// <returns>A collection of purchase orders from the catalog.</returns>
    Task<IEnumerable<PurchaseOrder>> FindByCatalogAsync(CatalogId catalogId);

    /// <summary>
    /// Checks if a purchase order exists by its order code.
    /// </summary>
    /// <param name="orderCode">The order code to check.</param>
    /// <returns>True if the order exists, otherwise false.</returns>
    Task<bool> ExistsByOrderCodeAsync(string orderCode);
}