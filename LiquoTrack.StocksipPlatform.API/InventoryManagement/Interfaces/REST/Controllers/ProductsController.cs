using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling product-related requests.
/// </summary>
/// <param name="productCommandService">
///     The service for handling product-related commands.
/// </param>
/// <param name="productQueryService">
///     The service for handling product-related queries.
/// </param>
/// <param name="careGuideQueryService">
///     The service for handling care guide-related queries.
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for products.")]
public class ProductsController(
        IProductCommandService productCommandService,
        IProductQueryService productQueryService,
        ICareGuideQueryService careGuideQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of a product by its ID.
    /// </summary>
    /// <param name="id">
    ///     The route parameter representing the unique identifier of the product to be retrieved.
    /// </param>
    /// <returns>
    ///     The product with the specified ID, or a 404 Not Found response if the product does not exist.
    /// </returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get product by ID.",
        Description = "Retrieves a product by its unique identifier.",
        OperationId = "GetProductById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product returned successfully.", typeof(ProductResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product with the specified ID was not found.")]
    public async Task<IActionResult> GetProductById([FromRoute] string id)
    {
        var getProductByIdQuery = new GetProductByIdQuery(ObjectId.Parse(id));
        var product = await productQueryService.Handle(getProductByIdQuery);
        if (product is null) return NotFound($"Product with ID {id} not found.");
        var productResource = ProductResourceFromEntityAssembler.ToResourceFromEntity(product);
        return Ok(productResource);
    }
    
    /// <summary>
    ///     Endpoint to retrieve the care guide associated with a product.
    /// </summary>
    /// <param name="id">
    ///     The unique identifier of the product whose care guide is requested.
    /// </param>
    /// <returns>
    ///     The care guide linked to the specified product, or a 404 Not Found response if none exists.
    /// </returns>
    [HttpGet("{id}/care-guides")]
    [SwaggerOperation(
        Summary = "Get care guide by product ID.",
        Description = "Retrieves the care guide assigned to a specific product.",
        OperationId = "GetCareGuideByProductId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Care guide returned successfully.", typeof(CareGuideResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Care guide for the specified product ID was not found.")]
    public async Task<IActionResult> GetCareGuideByProductId([FromRoute] string id)
    {
        var careGuide = await careGuideQueryService.Handle(new GetCareGuideByProductIdQuery(id));
        if (careGuide is null) return NotFound($"Care guide for product ID {id} not found.");
        var resource = CareGuideResourceFromEntityAssembler.ToResourceFromEntity(careGuide);
        return Ok(resource);
    }

    /// <summary>
    ///     Endpoint to handle the retrieval of all products by a given supplier id.
    /// </summary>
    /// <param name="supplierId">
    ///     The query parameter representing the unique identifier of the supplier whose products are to be retrieved.
    /// </param>
    /// <returns>
    ///     A list of products supplied by the specified supplier, or a 404 Not Found response if no products are found for the given supplier ID.
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all products by a given supplier id.",
        Description = "Retrieves all the products in a store supplied by a specific account.",
        OperationId = "GetAllProductsBySupplierId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Products retrieved successfully.", typeof(IEnumerable<ProductResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No products found for the specified supplier ID.")]
    public async Task<IActionResult> GetAllProductsBySupplierId([FromQuery] string supplierId)
    {
        var getAllProductsBySupplierIdQuery = new GetAllProductsBySupplierIdQuery(new AccountId(supplierId));
        var products = await productQueryService.Handle(getAllProductsBySupplierIdQuery);
        if (products.Count == 0) return NotFound($"No products found for supplier ID {supplierId}.");
        var productResources = products.Select(ProductResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(productResources);
    }

    /// <summary>
    ///     Endpoint to handle the update of product information.
    /// </summary>
    /// <param name="id">
    ///     The route parameter representing the unique identifier of the product to be updated.
    /// </param>
    /// <param name="resource">
    ///     The request body containing the updated product information.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the updated product details, or a 400 Bad Request response if the product could not be updated.
    /// </returns>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Update product information.",
        Description = "Updates the information of an existing product.",
        OperationId = "UpdateProduct")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product updated successfully.", typeof(ProductResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Product with the specified ID could not be updated.")]
    public async Task<IActionResult> UpdateProduct([FromRoute] string id,
        [FromForm] UpdateProductInformationResource resource)
    {
        var updateProductInformationCommand = UpdateProductInformationCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var product = await productCommandService.Handle(updateProductInformationCommand);
        if (product is null) return BadRequest($"Product with ID {id} could not be updated.");
        var updatedResource = ProductResourceFromEntityAssembler.ToResourceFromEntity(product);
        return Ok(updatedResource);
    }

    /// <summary>
    ///     Endpoint to handle the update of a product's minimum stock level.
    /// </summary>
    /// <param name="id">
    ///     The route parameter representing the unique identifier of the product whose minimum stock level is to be updated.
    /// </param>
    /// <param name="resource">
    ///     The request body containing the new minimum stock level.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the updated product details, or a 400 Bad Request response if the product's minimum stock level could not be updated.
    /// </returns>
    [HttpPatch("{id}/minimum-stock")]
    [SwaggerOperation(
        Summary = "Update product minimum stock.",
        Description = "Updates the minimum stock level of an existing product.",
        OperationId = "UpdateProductMinimumStock")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product minimum stock updated successfully.", typeof(ProductResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Product with the specified ID could not be updated.")]
    public async Task<IActionResult> UpdateProductMinimumStock([FromRoute] string id,
        [FromBody] UpdateProductMinimumStockResource resource)
    {
        var updateProductMinimumStockCommand = UpdateProductMinimumStockCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var product = await productCommandService.Handle(updateProductMinimumStockCommand);
        if (product is null) return BadRequest($"Product with ID {id} could not be updated.");
        var updatedResource = ProductResourceFromEntityAssembler.ToResourceFromEntity(product);
        return Ok(updatedResource);
    }
    
    /// <summary>
    ///     Endpoint to handle the deletion of a product by its ID.
    /// </summary>
    /// <param name="id">
    ///     The route parameter representing the unique identifier of the product to be deleted.
    /// </param>
    /// <returns>
    ///     A 204 No Content response if the product was successfully deleted, or a 400 Bad Request response if the product to delete could not be found.
    /// </returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Delete a product by ID.",
        Description = "Deletes a product from the store by its unique identifier.",
        OperationId = "DeleteProduct")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Product deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Product to delete could not be found.")]
    public async Task<IActionResult> DeleteProduct([FromRoute] string id)
    {
        var deleteProductCommand = new DeleteProductCommand(ObjectId.Parse(id));
        await productCommandService.Handle(deleteProductCommand);
        return NoContent();
    }
}