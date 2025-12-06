namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Resource for updating product information.
/// </summary>
public record UpdateProductInformationResource(
        string? Name,
        decimal? UnitPrice,
        string? Code,
        int? MinimumStock,
        decimal? Quantity,
        IFormFile? Image
    );