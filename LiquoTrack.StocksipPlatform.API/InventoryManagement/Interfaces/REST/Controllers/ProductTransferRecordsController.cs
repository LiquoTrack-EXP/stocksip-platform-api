using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller class for handling product-transfer-related requests.
/// </summary>
/// <param name="productTransferQueryService">
///     The service for handling product-transfer-related queries.
/// </param>
[ApiController]
[Route("api/v1/products/{productId}/transfers")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Products")]
public class ProductTransferRecordsController(
    IProductTransferQueryService productTransferQueryService
) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of all product transfer records for a specific product.
    /// </summary>
    /// <param name="productId">
    ///     The route parameter representing the unique identifier of the product for which to retrieve transfer records.
    /// </param>
    /// <returns>
    ///     The list of product transfer records for the specified product, or a 404 Not Found response if no transfer records are found for the given product ID.
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all product transfers by product ID",
        Description = "Retrieves all product transfer records associated with a specific product ID.",
        OperationId = "GetAllProductTransfersByProductId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product transfer records found!", typeof(IEnumerable<ProductTransferResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Transfer records for the specified product ID could not be found.")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> GetAllProductTransfersByProductId([FromRoute] string productId)
    {
        if (!ObjectId.TryParse(productId, out var productObjId)) return BadRequest("Invalid product ID.");

        var getAllProductTransfersByProductIdQuery = new GetAllProductTransfersByProductIdQuery(productObjId);
        var productTransfers = await productTransferQueryService.Handle(getAllProductTransfersByProductIdQuery);
        var enumerable = productTransfers as ProductTransfer[] ?? productTransfers.ToArray();
        if (enumerable.Length == 0) return NotFound($"Transfer records for product ID {productId} could not be found.");
        var resources = enumerable.Select(ProductTransferResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}