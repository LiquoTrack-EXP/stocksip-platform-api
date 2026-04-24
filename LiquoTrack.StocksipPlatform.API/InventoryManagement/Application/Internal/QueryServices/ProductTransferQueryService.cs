using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.QueryServices;

/// <summary>
///     Service for handling product-transfer-related queries.
/// </summary>
public class ProductTransferQueryService(
    IProductTransferRepository productTransferRepository
) : IProductTransferQueryService
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
    public async Task<IEnumerable<ProductTransfer>> Handle(GetAllProductTransfersByWarehouseIdQuery query)
    {
        return await productTransferRepository.GetAllByWarehouseIdAsync(query.WarehouseId);
    }

    /// <summary>
    ///     Query handler for retrieving all product transfers for a given product ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID for which to retrieve product transfers.
    /// </param>
    /// <returns>
    ///     A list of product transfers for the specified product ID or an empty list if no transfers are found.
    /// </returns>
    public async Task<IEnumerable<ProductTransfer>> Handle(GetAllProductTransfersByProductIdQuery query)
    {
        return await productTransferRepository.GetAllByProductIdAsync(query.ProductId);
    }

    /// <summary>
    ///     Query handler for retrieving a product transfer by product ID and warehouse ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the product ID and warehouse ID for which to retrieve the product transfer.
    /// </param>
    /// <returns>
    ///     A product transfer if found; otherwise, null.
    /// </returns>
    public async Task<ProductTransfer?> Handle(GetProductTransferByProductIdAndWarehouseIdQuery query)
    {
        return await productTransferRepository.GetByProductIdWarehouseIdAsync(query.ProductId, query.WarehouseId, query.ExpirationDate);
    }

    /// <summary>
    ///     Query handler for retrieving a product transfer by ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the ID of the product transfer to be retrieved.
    /// </param>
    /// <returns>
    ///     A product transfer if found; otherwise, null.
    /// </returns>
    public async Task<ProductTransfer?> Handle(GetProductTransferByIdQuery query)
    {
        return await productTransferRepository.FindByIdAsync(query.ProductTransferId.ToString());
    }
}