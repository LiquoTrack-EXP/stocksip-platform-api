using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller class for handling inventory-related requests.
/// </summary>
/// <param name="inventoryCommandService">
///     The service for handling inventory-related commands.
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for inventories.")]
public class InventoriesController(
        IInventoryCommandService inventoryCommandService,
        IInventoryQueryService inventoryQueryService,
        IProductQueryService productQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of an inventory by its ID.
    /// </summary>
    /// <param name="inventoryId">
    ///     The route parameter representing the unique identifier of the inventory to be retrieved.
    /// </param>
    /// <returns>
    ///     The inventory with the specified ID, or a 404 Not Found response if the inventory does not exist.
    /// </returns>
    [HttpGet("{inventoryId}")]
    [SwaggerOperation(
        Summary = "Get inventory by ID.",
        Description = "Retrieves an inventory by its unique identifier.",
        OperationId = "GetInventoryById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Inventory retrieved successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Inventory with the specified ID was not found.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryWithProductResource))]
    public async Task<IActionResult> GetInventoryById([FromRoute] string inventoryId)
    {
        ObjectId inventoryObjId = new(inventoryId);
        var getInventoryByIdQuery = new GetInventoryByIdQuery(inventoryObjId);
        var inventory = await inventoryQueryService.Handle(getInventoryByIdQuery);
        if (inventory is null) return NotFound($"Inventory with ID {inventoryId} not found...");
        var product = await productQueryService.Handle(new GetProductByIdQuery(inventory.ProductId));
        if (product is null) return NotFound($"Product with ID {inventory.ProductId} not found...");
        var inventoryResource = InventoryWithProductResourceFromEntityAssembler.ToResourceFromEntity(inventory, product); 
        return Ok(inventoryResource);
    }
    
    /// <summary>
    ///     Endpoint to handle the deletion of an inventory by its ID.
    /// </summary>
    /// <param name="inventoryId">
    ///     The route parameter representing the unique identifier of the inventory to be deleted.
    /// </param>
    /// <returns>
    ///     A 204 No Content response if the inventory was deleted successfully, or a 404 Not Found response if the inventory was not found.
    /// </returns>
    [HttpDelete("{inventoryId}")]
    [SwaggerOperation(
        Summary = "Delete inventory by ID.",
        Description = "Deletes an inventory by its unique identifier.",
        OperationId = "DeleteInventoryById")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Inventory deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Inventory with the specified ID was not found.")]
    public async Task<IActionResult> DeleteInventory([FromRoute] string inventoryId)
    {
        ObjectId inventoryObjId = new(inventoryId);
        var deleteInventoryCommand = new DeleteInventoryCommand(inventoryObjId);
        await inventoryCommandService.Handle(deleteInventoryCommand);
        return NoContent();
    }
}