using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Static assembler class to convert a collection of Product entities to a ProductsSummaryResource.
/// </summary>
public static class ProductsSummaryResourceFromEntityAssembler
{
    /// <summary>
    ///     Static method to convert a collection of Product entities to a ProductsSummaryResource.
    /// </summary>
    /// <param name="products">
    ///     The collection of Product entities to convert.
    /// </param>
    /// <param name="currentTotal">
    ///     The current total of products.
    /// </param>
    /// <param name="maxAllowed">
    ///     The maximum allowed number of products.
    /// </param>
    /// <returns>
    ///     A new ProductsSummaryResource representation of the Product entities.
    /// </returns>
    public static ProductsSummaryResource ToResourceFromEntity(ICollection<Product> products, int currentTotal, int? maxAllowed)
    {
        var productResource = products.Select(ProductResourceFromEntityAssembler.ToResourceFromEntity).ToList();
        return new ProductsSummaryResource(productResource, currentTotal, maxAllowed);
    }
}