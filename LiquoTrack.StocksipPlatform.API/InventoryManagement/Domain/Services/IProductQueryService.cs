using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

/// <summary>
///     Service interface for handling product-related queries.
/// </summary>
public interface IProductQueryService
{
    /// <summary>
    ///     Method to handle the retrieval of all products associated with a specific account ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID for which products are to be retrieved.
    /// </param>
    /// <returns>
    ///     The list of products associated with the specified account ID.
    ///     Or an empty collection if no products are found.
    /// </returns>
    Task<(ICollection<Product>, int currentTotal, int? planLimit)> Handle(GetAllProductsByAccountIdQuery query);

    /// <summary>
    ///     Method to handle the retrieval of all products associated with a specific supplier ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the supplier ID for which products are to be retrieved.
    /// </param>
    /// <returns>
    ///     The list of products associated with the specified supplier ID.
    ///     Or an empty collection if no products are found.
    /// </returns>
    Task<ICollection<Product>> Handle(GetAllProductsBySupplierIdQuery query);

    /// <summary>
    ///     Method to handle the retrieval of all products associated with a specific warehouse ID.   
    /// </summary>
    /// <param name="query">
    ///     The query object containing the warehouse ID for which products are to be retrieved. 
    /// </param>
    /// <returns>
    ///     A list of products associated with the specified warehouse ID.
    ///     Or an empty collection if no products are found.
    /// </returns>
    Task<ICollection<Product>> Handle(GetAllProductsByWarehouseIdQuery query);
    
    /// <summary>
    ///     Method to handle the retrieval of a product by its ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the ID of the product to retrieve.
    /// </param>
    /// <returns>
    ///     The product with the specified ID.
    ///     Or null if no product is found.   
    /// </returns>
    Task<Product?> Handle(GetProductByIdQuery query);
}