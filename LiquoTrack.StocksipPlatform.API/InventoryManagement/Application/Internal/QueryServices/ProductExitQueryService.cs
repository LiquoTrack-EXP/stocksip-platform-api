using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.QueryServices;

/// <summary>
///     Implementation of the <see cref="IProductExitQueryService"/> interface.
/// </summary>
/// <param name="productExitRepository">
///     The repository for handling the ProductExits in the database.
/// </param>
public class ProductExitQueryService(
    IProductExitRepository productExitRepository
) : IProductExitQueryService
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
    public async Task<IEnumerable<ProductExit>> Handle(GetAllProductExitsByWarehouseIdQuery query)
    {
        return await productExitRepository.GetAllByWarehouseIdAsync(query.WarehouseId);
    }

    /// <summary>
    ///     Handler for the retrieval of all product exits for a given product ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID for which product exits are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of product exits for the specified product ID, if any; otherwise, an empty list. 
    /// </returns>
    public async Task<IEnumerable<ProductExit>> Handle(GetAllProductExitsByProductIdQuery query)
    {
        return await productExitRepository.GetAllByProductIdAsync(query.ProductId);
    }

    /// <summary>
    ///     Handler for the retrieval of a product exit by product ID and warehouse ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID and warehouse ID for which the product exit is to be retrieved.
    /// </param>
    /// <returns>
    ///     The product exit if found; otherwise, null.
    /// </returns>
    public async Task<ProductExit?> Handle(GetProductExitByProductIdAndWarehouseIdQuery query)
    {
        return await productExitRepository.GetByProductIdAndWarehouseIdAsync(query.ProductId, query.WarehouseId, query.ExpirationDate);
    }

    /// <summary>
    ///     Handler for the retrieval of a product exit by ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the ID of the product exit to be retrieved.
    /// </param>
    /// <returns>
    ///     The product exit if found; otherwise, null.
    /// </returns>
    public async Task<ProductExit?> Handle(GetProductExitByIdQuery query)
    {
        return await productExitRepository.FindByIdAsync(query.ProductExitId.ToString());
    }
}