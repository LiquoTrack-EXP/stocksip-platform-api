using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.QueryServices;

/// <summary>
///     Service implementation for handling product-related queries.
/// </summary>
/// <param name="productRepository">
///     The repository for handling the Products in the database.
/// </param>
public class ProductQueryService(
        IProductRepository productRepository,
        IPaymentAndSubscriptionsFacade paymentAndSubscriptionsFacade
    ) : IProductQueryService
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
    ///     Also returns the current total number of products and the plan limit for the account.
    /// </returns>
    public async Task<(ICollection<Product>, int currentTotal, int? planLimit)> Handle(GetAllProductsByAccountIdQuery query)
    {
        var products = await productRepository.FindByAccountIdAsync(query.AccountId);
        var currentTotal = await productRepository.CountByAccountIdAsync(query.AccountId);
        var productLimit = await paymentAndSubscriptionsFacade.GetPlanProductsLimitByAccountId(query.AccountId.GetId);
        return (products, currentTotal, productLimit);
    }

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
    public async Task<ICollection<Product>> Handle(GetAllProductsBySupplierIdQuery query)
    {
        return await productRepository.FindBySupplierIdAsync(query.SupplierId);  
    }

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
    public async Task<ICollection<Product>> Handle(GetAllProductsByWarehouseIdQuery query)
    {
        return await productRepository.FindByWarehouseIdAsync(query.WarehouseId); 
    }

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
    public async Task<Product?> Handle(GetProductByIdQuery query)
    {
        return await productRepository.FindByIdAsync(query.ProductId.ToString());
    }
}