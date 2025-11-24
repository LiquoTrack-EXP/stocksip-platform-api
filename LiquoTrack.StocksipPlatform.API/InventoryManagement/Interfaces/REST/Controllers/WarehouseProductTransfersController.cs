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
///     Controller class for handling warehouse-product-transfer-related requests.
/// </summary>
/// <param name="productTransferQueryService">
///     The service for handling warehouse-product-transfer-related queries.
/// </param>
[ApiController]
[Route("api/v1/warehouses/{warehouseId}/product-transfers")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Warehouses")]
public class WarehouseProductTransfersController(
    IProductTransferQueryService productTransferQueryService
) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of all product transfer records for a specific warehouse.
    /// </summary>
    /// <param name="warehouseId">
    ///     The route parameter representing the unique identifier of the warehouse for which to retrieve product transfer records.
    /// </param>
    /// <returns>
    ///     The list of product transfer records for the specified warehouse, or a 404 Not Found response if no product transfer records are found for the given warehouse ID.
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all product transfers by warehouse ID",
        Description = "Retrieves all product transfer records associated with a specific warehouse ID.",
        OperationId = "GetAllProductTransfersByWarehouseId"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Product transfer records found!", typeof(IEnumerable<ProductTransferResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No product transfer records found for the specified warehouse ID.")]
    public async Task<IActionResult> GetAllProductTransfersByWarehouseId([FromRoute] string warehouseId)
    {
        if (!ObjectId.TryParse(warehouseId, out var warehouseObjId)) return BadRequest("Invalid warehouse ID.");

        var getAllProductTransfersByWarehouseIdQuery = new GetAllProductTransfersByWarehouseIdQuery(warehouseObjId);
        var productTransfers = await productTransferQueryService.Handle(getAllProductTransfersByWarehouseIdQuery);
        var enumerable = productTransfers as ProductTransfer[] ?? productTransfers.ToArray();
        if (enumerable.Length == 0) return NotFound($"Transfer records for warehouse ID {warehouseId} could not be found.");
        var resources = enumerable.Select(ProductTransferResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}