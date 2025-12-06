namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource for creating a catalog for a specific account.
/// </summary>
/// <param name="name">The catalog name.</param>
/// <param name="description">The catalog description.</param>
/// <param name="contactEmail">The contact email.</param>
public record CreateCatalogForAccountResource(string name, string description, string contactEmail);