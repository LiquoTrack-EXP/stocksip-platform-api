namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
/// Data Transfer Object representing essential product details exposed to other contexts.
/// </summary>
public class ProductDetailsResource
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int MinimumStock { get; set; } = 0;
}