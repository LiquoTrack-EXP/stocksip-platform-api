using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;

/// <summary>
/// Assembler to convert Account and Catalog domain entities into their corresponding REST resources.
/// </summary>
public static class AccountCatalogsResourceAssembler
{
    /// <summary>
    /// Converts domain entities of Account, Business, and a collection of Catalogs into an AccountCatalogsResource.
    /// </summary>
    /// <param name="account">The domain account entity.</param>
    /// <param name="business">The domain business entity associated with the account.</param>
    /// <param name="catalogs">The collection of domain catalog entities to be mapped.</param>
    /// <returns>A REST resource representing the account with its business and published catalogs.</returns>
    public static AccountCatalogsResource ToResource(
        Account account,
        Business business,
        IEnumerable<Catalog> catalogs
    )
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account), "Account cannot be null.");

        if (business == null)
            throw new ArgumentNullException(nameof(business), "Business cannot be null.");

        var businessResource = new BusinessResource(
            BusinessName: business.BusinessName?.Value ?? "Unnamed Business",
            BusinessEmail: business.BusinessEmail?.Value ?? "Not provided",
            Ruc: business.Ruc?.Value ?? "N/A"
        );

        var accountResource = new AccountWithBusinessResource(
            id: account.Id.ToString(),
            business: businessResource
        );

        var publishedCatalogs = (catalogs ?? Enumerable.Empty<Catalog>())
            .Where(c => c.IsPublished)
            .Select(c => new CatalogResource(
                id: c.Id.ToString(),
                name: c.Name,
                description: c.Description,
                catalogItems: c.CatalogItems?.Select(item => new CatalogItemResource(
                    ProductId: item.ProductId.GetId,
                    Amount: item.UnitPrice.GetAmount(),
                    Currency: item.UnitPrice.GetCurrencyCode(),
                    AddedDate: item.AddedAt,
                    ProductName: item.ProductName,
                    ProductImage: item.ImageUrl,
                    AvailableStock: item.AvailableStock
                )).ToList() ?? new List<CatalogItemResource>(),
                ownerAccount: c.OwnerAccount.GetId,
                contactEmail: c.ContactEmail?.Value ?? "Not provided",
                isPublished: c.IsPublished
            ));

        return new AccountCatalogsResource(accountResource, publishedCatalogs);
    }
}