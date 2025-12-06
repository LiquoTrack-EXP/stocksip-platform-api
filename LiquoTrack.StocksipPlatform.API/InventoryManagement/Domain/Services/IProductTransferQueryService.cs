using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

/// <summary>
///     Interface for handling product-transfer-related queries.
/// </summary>
public interface IProductTransferQueryService
{
    /// <summary>
    ///     Query handler for retrieving all product transfers for a given warehouse ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the warehouse ID for which to retrieve product transfers.
    /// </param>
    /// <returns>
    ///     A list of product transfers for the specified warehouse ID or an empty list if no transfers are found.
    /// </returns>
    Task<IEnumerable<ProductTransfer>> Handle(GetAllProductTransfersByWarehouseIdQuery query);
    
    /// <summary>
    ///     Query handler for retrieving all product transfers for a given product ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID for which to retrieve product transfers.
    /// </param>
    /// <returns>
    ///     A list of product transfers for the specified product ID or an empty list if no transfers are found.
    /// </returns>
    Task<IEnumerable<ProductTransfer>> Handle(GetAllProductTransfersByProductIdQuery query);
    
    /// <summary>
    ///     Query handler for retrieving a product transfer by product ID and warehouse ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID and warehouse ID for which to retrieve the product transfer.
    /// </param>
    /// <returns>
    ///     A product transfer if found; otherwise, null.
    /// </returns>
    Task<ProductTransfer?> Handle(GetProductTransferByProductIdAndWarehouseIdQuery query);
    
    /// <summary>
    ///     Query handler for retrieving a product transfer by ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the ID of the product transfer to be retrieved.
    /// </param>
    /// <returns>
    ///     A product transfer if found; otherwise, null.
    /// </returns>
    Task<ProductTransfer?> Handle(GetProductTransferByIdQuery query);
}