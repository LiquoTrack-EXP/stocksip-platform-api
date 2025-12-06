using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

/// <summary>
///     Interface for handling product exit-related queries.
/// </summary>
public interface IProductExitQueryService
{
    /// <summary>
    ///     Handler for the retrieval of all product exits for a given warehouse ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the warehouse ID for which product exits are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of product exits for the specified warehouse ID, if any; otherwise, an empty list.  
    /// </returns>
    Task<IEnumerable<ProductExit>> Handle(GetAllProductExitsByWarehouseIdQuery query);
    
    /// <summary>
    ///     Handler for the retrieval of all product exits for a given product ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID for which product exits are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of product exits for the specified product ID, if any; otherwise, an empty list. 
    /// </returns>
    Task<IEnumerable<ProductExit>> Handle(GetAllProductExitsByProductIdQuery query);
    
    /// <summary>
    ///     Handler for the retrieval of a product exit by product ID and warehouse ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID and warehouse ID for which the product exit is to be retrieved.
    /// </param>
    /// <returns>
    ///     The product exit if found; otherwise, null.
    /// </returns>
    Task<ProductExit?> Handle(GetProductExitByProductIdAndWarehouseIdQuery query);
    
    /// <summary>
    ///     Handler for the retrieval of a product exit by ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the ID of the product exit to be retrieved.
    /// </param>
    /// <returns>
    ///     The product exit if found; otherwise, null.
    /// </returns>
    Task<ProductExit?> Handle(GetProductExitByIdQuery query);
}