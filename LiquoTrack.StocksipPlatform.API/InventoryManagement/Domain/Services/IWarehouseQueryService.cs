using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

/// <summary>
///     Interface for querying warehouse information.
/// </summary>
public interface IWarehouseQueryService
{
    /// <summary>
    ///     Method to get a warehouse by its ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the warehouse ID.
    /// </param>
    /// <returns>
    ///     The warehouse if found; otherwise, null.
    /// </returns>
    Task<Warehouse?> Handle(GetWarehouseByIdQuery query);
    
    /// <summary>
    ///     Method to get all warehouses associated with a specific account ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     A collection of warehouses associated with the account ID. Or an empty collection if none are found.
    /// </returns>
    Task<(ICollection<Warehouse>, int currentTotal, int? planLimit)> Handle(GetAllWarehousesByAccountId query);
    
}