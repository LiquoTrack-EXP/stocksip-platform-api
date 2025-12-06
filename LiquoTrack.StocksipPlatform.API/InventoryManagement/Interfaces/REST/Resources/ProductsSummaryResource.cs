namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a collection of products along with the total count.
/// </summary>
/// <param name="Products">
///     The list of products.
/// </param>
/// <param name="Total">
///     The total count of products.
/// </param>
/// <param name="MaxProductsAllowed">
///     The maximum number of products allowed under the plan.
/// </param>
public record ProductsSummaryResource(List<ProductResource> Products, int Total, int? MaxProductsAllowed);