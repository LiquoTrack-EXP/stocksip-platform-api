using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;

/// <summary>
/// Assembler to convert Catalog entity to CatalogResource.
/// </summary>
public static class CatalogResourceFromEntityAssembler
{
    /// <summary>
    /// Converts a Catalog entity to a CatalogResource.
    /// </summary>
    /// <param name="entity">The catalog entity.</param>
    /// <returns>The catalog resource.</returns>
    public static CatalogResource ToResourceFromEntity(Catalog entity)
    {
        return new CatalogResource(
            entity.Id.ToString(),
            entity.Name,
            entity.Description,
            entity.CatalogItems.Select(item => new CatalogItemResource(
                item.ProductId.GetId,
                item.UnitPrice.GetAmount(),
                item.UnitPrice.GetCurrencyCode(),
                item.AddedAt,
                item.ProductName,
                item.ImageUrl,
                item.AvailableStock
            )).ToList(),
            entity.OwnerAccount.GetId,
            entity.ContactEmail.GetValue,
            entity.IsPublished
        );
    }

    /// <summary>
    /// Converts a collection of Catalog entities to CatalogResources.
    /// </summary>
    /// <param name="entities">The collection of catalog entities.</param>
    /// <returns>The collection of catalog resources.</returns>
    public static IEnumerable<CatalogResource> ToResourceFromEntity(IEnumerable<Catalog> entities)
    {
        return entities.Select(ToResourceFromEntity);
    }
}