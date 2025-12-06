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
///     Controller class for handling product-exit-related requests.
/// </summary>
/// <param name="productExitQueryService">
///     The service for handling product-exit-related queries.
/// </param>
[ApiController]
[Route("api/v1/products/{productId}/exits")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Products")]
public class ProductExitRecordsController(
        IProductExitQueryService productExitQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of all product exits for a specific product.
    /// </summary>
    /// <param name="productId">
    ///     The route parameter representing the unique identifier of the product for which to retrieve product exits.
    /// </param>
    /// <returns>
    ///     The list of product exits for the specified product, or a 404 Not Found response if no product exits are found for the given product ID.
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all product exits by product ID",
        Description = "Retrieves all product exit records associated with a specific product ID.",
        OperationId = "GetAllProductExitsByProductId"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Product exits found!", typeof(IEnumerable<ProductExitResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No product exits found for the specified product ID.")]
    public async Task<ActionResult> GetAllProductExitsByProductId([FromRoute] string productId)
    {
        if (!ObjectId.TryParse(productId, out var productObjId)) return BadRequest("Invalid product ID.");

        var getAllProductExitsByProductId = new GetAllProductExitsByProductIdQuery(productObjId);
        var productExits = await productExitQueryService.Handle(getAllProductExitsByProductId);
        var enumerable = productExits as ProductExit[] ?? productExits.ToArray();
        if (enumerable.Length == 0) return NotFound("No product exits found for the specified product ID.");
        var resources = enumerable.Select(ProductExitResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}