using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Assembler to convert a collection of Warehouse entities to a WarehousesWithCountResource.
/// </summary>
public class WarehousesSummaryResourceFromEntityAssembler
{
    /// <summary>
    ///     Method to convert a collection of Warehouse entities to a WarehousesWithCountResource.
    /// </summary>
    /// <param name="warehouses">
    ///     A collection of Warehouse entities to convert. 
    /// </param>
    /// <param name="currentTotal">
    ///     The current total of warehouses.
    /// </param>
    /// <param name="maxAllowed">
    ///     The maximum number of warehouses allowed.
    /// </param>
    /// <returns>
    ///     A WarehousesWithCountResource representation of the Warehouse entities.
    /// </returns>
    public static WarehousesSummaryResource ToResourceFromEntity(ICollection<Warehouse> warehouses, int currentTotal, int? maxAllowed)
    {
        var warehouseResource = warehouses.Select(WarehouseResourceFromEntityAssembler
                .ToResourceFromEntity)
                .ToList();
        return new WarehousesSummaryResource(warehouseResource, currentTotal, maxAllowed);
    }
}