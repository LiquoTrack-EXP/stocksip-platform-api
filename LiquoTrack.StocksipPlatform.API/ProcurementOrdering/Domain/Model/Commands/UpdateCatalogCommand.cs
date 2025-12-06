namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to update an existing catalog.
/// </summary>
/// <param name="catalogId">The identifier of the catalog to update.</param>
/// <param name="name">The new name of the catalog.</param>
/// <param name="description">The new description of the catalog.</param>
/// <param name="contactEmail">The new contact email for the catalog.</param>
public record UpdateCatalogCommand(string catalogId, string name, string description, string contactEmail);