using System.Net.Mime;
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
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for product exit records.")]
public class ProductExitsController(
        IProductExitQueryService productExitQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of a product exit by its ID.
    /// </summary>
    /// <param name="id">
    ///     The route parameter representing the unique identifier of the product exit to be retrieved.
    /// </param>
    /// <returns>
    ///     The product exit with the specified ID, or a 404 Not Found response if the product exit does not exist.
    /// </returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get A Product Exit By ID",
        Description = "Fetches a product exit record by its unique identifier.",
        OperationId = "GetProductExitById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductExitResource))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> GetProductExitById([FromRoute] string id)
    {
        if (!ObjectId.TryParse(id, out var productExitObjId)) return BadRequest("Invalid product exit ID.");
        
        var getProductExitByIdQuery = new GetProductExitByIdQuery(productExitObjId);
        var productExit = await productExitQueryService.Handle(getProductExitByIdQuery);
        if (productExit is null) return NotFound("Product exit could not be found.");
        var resource = ProductExitResourceFromEntityAssembler.ToResourceFromEntity(productExit);
        return Ok(resource);
    }
}