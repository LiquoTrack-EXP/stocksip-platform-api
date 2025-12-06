using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling product-related requests.
/// </summary>
/// <param name="productQueryService">
///     The service for handling product-related queries. 
/// </param>
[ApiController]
[Route("api/v1/accounts/{accountId}/products")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
public class AccountProductsController(
        IProductCommandService productCommandService,
        IProductQueryService productQueryService   
    ) : ControllerBase
{
    
    /// <summary>
    ///     Endpoint to handle the retrieval of all products for a specific account.
    /// </summary>
    /// <param name="accountId">
    ///     The route parameter representing the unique identifier of the account for which to retrieve products.   
    /// </param>
    /// <returns>
    ///     A list of products for the specified account, or a 404 Not Found response if no products are found for the given account ID. 
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get All Products by Account ID",
        Description = "Retrieves a list of products by a specific Account ID.",
        OperationId = "GetAllProductsByAccountId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Products found!", typeof(ProductsSummaryResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Products not found for the give Account ID...")]
    public async Task<IActionResult> GetAllProductsByAccountId([FromRoute] string accountId)
    {
        var targetAccountId = new AccountId(accountId);
        var getAllProductsByAccountIdQuery = new GetAllProductsByAccountIdQuery(targetAccountId);
        var (products, currentTotal, productLimit) = await productQueryService.Handle(getAllProductsByAccountIdQuery);
        var resource = ProductsSummaryResourceFromEntityAssembler.ToResourceFromEntity(products, currentTotal, productLimit);
        return Ok(resource);
    }
    
    /// <summary>
    ///     Endpoint to handle the registration of a new product.
    /// </summary>
    /// <param name="resource">
    ///     The request body containing the details of the product to be registered.
    /// </param>
    /// <param name="accountId">
    ///     The route parameter representing the unique identifier of the account for which the product is to be registered. 
    /// </param>
    /// <returns>
    ///     A 201 Created response with the details of the newly registered product, or a 400 Bad Request response if the product could not be registered.
    /// </returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Register a new product.",
        Description = "Registers a new product in the store.",
        OperationId = "RegisterProduct")]
    [SwaggerResponse(StatusCodes.Status201Created, "Product registered successfully.", typeof(ProductResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Product could not be registered.")]
    public async Task<IActionResult> RegisterProduct([FromForm] RegisterProductResource resource, [FromRoute] string accountId)
    {
        var registerProductCommand = RegisterProductCommandFromResourceAssembler.ToCommandFromResource(resource, accountId);
        var product = await productCommandService.Handle(registerProductCommand);
        if (product is null) return BadRequest("Product could not be registered.");
        var productResource = ProductResourceFromEntityAssembler.ToResourceFromEntity(product);
        return Ok(productResource);
    }
}