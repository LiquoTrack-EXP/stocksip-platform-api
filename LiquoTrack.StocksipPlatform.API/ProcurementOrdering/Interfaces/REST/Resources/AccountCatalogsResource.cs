namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource representing account and its published catalogs.
/// </summary>
/// <param name="account">The account with its business info.</param>
/// <param name="catalogs">The list of published catalogs owned by the account.</param>
public record AccountCatalogsResource(
    AccountWithBusinessResource account,
    IEnumerable<CatalogResource> catalogs
);