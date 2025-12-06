namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource for creating a catalog.
/// </summary>
/// <param name="name">The catalog name.</param>
/// <param name="description">The catalog description.</param>
/// <param name="ownerAccount">The owner account identifier.</param>
/// <param name="contactEmail">The contact email.</param>
public record CreateCatalogResource(
    string name,
    string description,
    string ownerAccount,
    string contactEmail
);
