namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to create a new catalog.
/// </summary>
/// <param name="name">The catalog name.</param>
/// <param name="description">The catalog description.</param>
/// <param name="ownerAccount">The owner account identifier.</param>
/// <param name="contactEmail">The contact email.</param>
/// <param name="warehouseId">The warehouse identifier (optional).</param>
public record CreateCatalogCommand(
    string name,
    string description,
    string ownerAccount,
    string contactEmail,
    string? warehouseId = null
);
