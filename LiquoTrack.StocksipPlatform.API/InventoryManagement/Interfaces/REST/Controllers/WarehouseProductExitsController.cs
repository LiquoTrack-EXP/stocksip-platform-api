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
///     Controller class for handling warehouse-product-exit-related requests.
/// </summary>
/// <param name="productExitQueryService">
///     The service for handling warehouse-product-exit-related queries.
/// </param>
[ApiController]
[Route("api/v1/warehouses/{warehouseId}/product-exits")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Warehouses")]
public class WarehouseProductExitsController(
        IProductExitQueryService productExitQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of all product exits for a specific warehouse.
    /// </summary>
    /// <param name="warehouseId">
    ///     The route parameter representing the unique identifier of the warehouse for which to retrieve product exits.
    /// </param>
    /// <returns>
    ///     The list of product exits for the specified warehouse, or a 404 Not Found response if no product exits are found for the given warehouse ID.
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all product exits by warehouse ID",
        Description = "Retrieves all product exit records associated with a specific warehouse ID.",
        OperationId = "GetAllProductExitsByWarehouseId"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Product exits found!", typeof(IEnumerable<ProductExitResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No product exits found for the specified warehouse ID.")]
    public async Task<ActionResult> GetAllProductExitsByWarehouseId([FromRoute] string warehouseId)
    {
        if (!ObjectId.TryParse(warehouseId, out var warehouseObjId)) return BadRequest("Invalid warehouse ID.");

        var getAllProductExitsByWarehouseId = new GetAllProductExitsByWarehouseIdQuery(warehouseObjId);
        var productExits = await productExitQueryService.Handle(getAllProductExitsByWarehouseId);
        var enumerable = productExits as ProductExit[] ?? productExits.ToArray();
        if (enumerable.Length == 0) return NotFound("No product exits found for the specified warehouse ID.");
        var resources = enumerable.Select(ProductExitResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}