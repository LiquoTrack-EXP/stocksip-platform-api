using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling warehouse-related requests.
/// </summary>
/// <param name="warehouseCommandService">
///     The service for handling warehouse-related commands.
/// </param>
/// <param name="warehouseQueryService">
///     The service for handling warehouse-related queries.
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for warehouses.")]
public class WarehousesController(
        IWarehouseCommandService warehouseCommandService,
        IWarehouseQueryService warehouseQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of a warehouse by its ID.
    /// </summary>
    /// <param name="warehouseId">
    ///     The route parameter representing the unique identifier of the warehouse to be retrieved.
    /// </param>
    /// <returns>
    ///     The warehouse with the specified ID, or a 404 Not Found response if the warehouse does not exist.
    /// </returns>
    [HttpGet("{warehouseId}")]
    [SwaggerOperation(
        Summary = "Get warehouse by ID.",
        Description = "Retrieves a warehouse by its unique identifier.",
        OperationId = "GetWarehouseById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouse returned successfully.", typeof(WarehouseResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouse with the specified ID was not found.")]
    public async Task<IActionResult> GetWarehouseById([FromRoute] string warehouseId)
    {
        var getWarehouseById = new GetWarehouseByIdQuery(warehouseId);
        var warehouse = await warehouseQueryService.Handle(getWarehouseById);
        if (warehouse is null) return NotFound($"Warehouse with ID {warehouseId} not found.");
        var warehouseResource = WarehouseResourceFromEntityAssembler.ToResourceFromEntity(warehouse);
        return Ok(warehouseResource);
    }

    /// <summary>
    ///     Endpoint to handle the update of a warehouse.
    /// </summary>
    /// <param name="warehouseId">
    ///     The route parameter representing the unique identifier of the warehouse to be updated.
    /// </param>
    /// <param name="resource">
    ///     The request body containing the updated warehouse information.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the updated warehouse details, or a 400 Bad Request response if the warehouse could not be updated.
    /// </returns>
    [HttpPut("{warehouseId}")]
    [SwaggerOperation(
        Summary = "Update warehouse information.",
        Description = "Updates the information of a warehouse with the provided details.",
        OperationId = "UpdateWarehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouse updated successfully.", typeof(WarehouseResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouse with the specified ID was not found.")]
    public async Task<IActionResult> UpdateWarehouse([FromRoute] string warehouseId,
        [FromForm] UpdateWarehouseInformationResource resource)
    {
        var updateWarehouseCommand = UpdateWarehouseInformationCommandFromResourceAssembler.ToCommandFromResource(warehouseId, resource);
        var warehouse = await warehouseCommandService.Handle(updateWarehouseCommand);
        if (warehouse is null) return NotFound($"Warehouse with ID {warehouseId} not found.");
        var warehouseResource = WarehouseResourceFromEntityAssembler.ToResourceFromEntity(warehouse);
        return Ok(warehouseResource);
    }

    /// <summary>
    ///     Endpoint to handle the deletion of a warehouse by its ID.
    /// </summary>
    /// <param name="warehouseId">
    ///     The route parameter representing the unique identifier of the warehouse to be deleted.
    /// </param>
    /// <returns>
    ///     A 204 No Content response if the warehouse was successfully deleted, or a 404 Not Found response if the warehouse to delete could not be found.   
    /// </returns>
    [HttpDelete("{warehouseId}")]
    [SwaggerOperation(
        Summary = "Delete a warehouse.",
        Description = "Deletes a warehouse with the specified ID.",
        OperationId = "DeleteWarehouse")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Warehouse deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouse with the specified ID was not found.")]
    public async Task<IActionResult> DeleteWarehouse([FromRoute] string warehouseId)
    {
        var deleteWarehouseCommand = new DeleteWarehouseCommand(warehouseId);
        await warehouseCommandService.Handle(deleteWarehouseCommand);
        return NoContent();
    }
}