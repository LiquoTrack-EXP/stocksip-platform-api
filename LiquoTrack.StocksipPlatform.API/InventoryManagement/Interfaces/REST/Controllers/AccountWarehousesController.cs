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
///     Controller for handling warehouse-related requests for a specific account.
/// </summary>
/// <param name="warehouseQueryService">
///     The service for handling warehouse-related queries.
/// </param>
[ApiController]
[Route("api/v1/accounts/{accountId}/warehouses")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
public class AccountWarehousesController(
        IWarehouseCommandService warehouseCommandService,
        IWarehouseQueryService warehouseQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of all warehouses for a specific account. 
    /// </summary>
    /// <param name="accountId">
    ///     The route parameter representing the unique identifier of the account for which to retrieve warehouses.
    /// </param>
    /// <returns>
    ///     A list of warehouses for the specified account, or a 404 Not Found response if no warehouses are found for the given account ID.
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get All Warehouses by Account ID",
        Description = "Retrieves a list of warehouses by a specific Account ID.",
        OperationId = "GetAllWarehousesByAccountId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Warehouses retrieved successfully.", typeof(WarehousesSummaryResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Warehouses not found for the give Account ID...")]
    public async Task<IActionResult> GetAllWarehousesByAccountId([FromRoute] string accountId)
    {
        var targetAccountId = new AccountId(accountId);
        var getAllWarehousesAndCountByAccountIdQuery = new GetAllWarehousesByAccountId(targetAccountId);
        var (warehouses, currentTotal, warehouseLimit) = await warehouseQueryService.Handle(getAllWarehousesAndCountByAccountIdQuery);
        var resource = WarehousesSummaryResourceFromEntityAssembler.ToResourceFromEntity(warehouses, currentTotal, warehouseLimit);
        return Ok(resource);
    }
    
    /// <summary>
    ///     Endpoint to handle the registration of a new warehouse.
    /// </summary>
    /// <param name="resource">
    ///     The request body containing the details of the warehouse to be registered.
    /// </param>
    /// <returns>
    ///     A 201 Created response with the details of the newly registered warehouse, or a 400 Bad Request response if the warehouse could not be registered.   
    /// </returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Register a new warehouse.",
        Description = "Registers a new warehouse with the provided details.",
        OperationId = "RegisterWarehouse")]
    [SwaggerResponse(StatusCodes.Status201Created, "Warehouse registered successfully.", typeof(WarehouseResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data.")]
    public async Task<IActionResult> RegisterWarehouse([FromForm] RegisterWarehouseResource resource, [FromRoute] string accountId)
    {
        var registerWarehouseCommand = RegisterWarehouseCommandFromResourceAssembler.ToCommandFromResource(resource, accountId);
        var warehouse = await warehouseCommandService.Handle(registerWarehouseCommand);
        if (warehouse is null) return BadRequest("Warehouse could not be registered.");
        var warehouseResource = WarehouseResourceFromEntityAssembler.ToResourceFromEntity(warehouse);
        return Ok(warehouseResource);
    }
}