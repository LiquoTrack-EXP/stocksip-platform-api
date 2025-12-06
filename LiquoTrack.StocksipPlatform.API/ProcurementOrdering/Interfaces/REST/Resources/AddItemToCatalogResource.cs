namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource for adding an item to a catalog.
/// </summary>
public record AddItemToCatalogResource
{
    /// <summary>
    /// The product identifier.
    /// </summary>
    public string ProductId { get; init; } = string.Empty;

    /// <summary>
    /// The warehouse identifier where the product is stored.
    /// </summary>
    public string WarehouseId { get; init; } = string.Empty;

    /// <summary>
    /// The available stock of the product in the specified warehouse.
    /// </summary>
    public int Stock { get; init; }
}