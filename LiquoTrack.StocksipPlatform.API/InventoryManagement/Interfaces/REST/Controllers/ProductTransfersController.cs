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
///     Controller class for handling product-transfer-related requests.
/// </summary>
/// <param name="productTransferQueryService">
///     The service for handling product-transfer-related queries.
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for product transfer records.")]
public class ProductTransfersController(
    IProductTransferQueryService productTransferQueryService
) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of a product transfer by its ID.
    /// </summary>
    /// <param name="id">
    ///     The route parameter representing the unique identifier of the product transfer to be retrieved.
    /// </param>
    /// <returns>
    ///     The product transfer with the specified ID, or a 404 Not Found response if the product transfer does not exist.
    /// </returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get A Product Transfer By ID",
        Description = "Fetches a product transfer record by its unique identifier.",
        OperationId = "GetProductTransferById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductTransferResource))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> GetProductTransferById([FromRoute] string id)
    {
        if (!ObjectId.TryParse(id, out var productTransferObjId)) return BadRequest("Invalid product transfer ID.");

        var getProductTransferByIdQuery = new GetProductTransferByIdQuery(productTransferObjId);
        var productTransfer = await productTransferQueryService.Handle(getProductTransferByIdQuery);
        if (productTransfer is null) return NotFound($"Product transfer with ID {id} not found...");
        var resource = ProductTransferResourceFromEntityAssembler.ToResourceFromEntity(productTransfer);
        return Ok(resource);
    }
}