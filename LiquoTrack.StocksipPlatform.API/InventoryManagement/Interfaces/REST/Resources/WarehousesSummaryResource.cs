namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a collection of warehouses along with the total count.
/// </summary>
/// <param name="Total">
///     The total count of warehouses.
/// </param>
/// <param name="MaxWarehousesAllowed">
///     The maximum number of warehouses allowed under the plan.
/// </param>
/// <param name="Warehouses">
///     A collection of warehouses.
/// </param>
public record WarehousesSummaryResource(List<WarehouseResource> Warehouses, int Total, int? MaxWarehousesAllowed);