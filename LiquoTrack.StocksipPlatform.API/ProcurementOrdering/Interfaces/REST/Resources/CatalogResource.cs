namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource representing a catalog.
/// </summary>
/// <param name="id">The unique identifier of the catalog.</param>
/// <param name="name">The name of the catalog.</param>
/// <param name="description">The description of the catalog.</param>
/// <param name="catalogItems">The list of items in the catalog.</param>
/// <param name="ownerAccount">The owner account identifier.</param>
/// <param name="contactEmail">The contact email.</param>
/// <param name="isPublished">Indicates if the catalog is published.</param>
public record CatalogResource(
    string id,
    string name,
    string description,
    List<CatalogItemResource> catalogItems,
    string ownerAccount,
    string contactEmail,
    bool isPublished
);