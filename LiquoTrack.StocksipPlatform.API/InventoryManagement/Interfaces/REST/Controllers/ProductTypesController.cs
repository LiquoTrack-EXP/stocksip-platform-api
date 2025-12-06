using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling product-type-related requests.
/// </summary>
/// <param name="typeQueryService">
///     The service for handling product-type-related queries.
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for product types.")]
public class ProductTypesController(
        ITypeQueryService typeQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of all product types.
    /// </summary>
    /// <returns>
    ///     A list of product types, or a 404 Not Found response if no product types are found.
    /// </returns>
    [AllowAnonymous]
    [HttpGet]
    [SwaggerOperation(Summary = "Get all product types.",
        Description = "Retrieves a list of all product types.",
        OperationId = "GetAllProductTypes")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product types retrieved successfully.", typeof(IEnumerable<TypeResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product types could not be retrieved.")]
    public async Task<IActionResult> GetAllProductTypes()
    {
        var getAllTypesQuery = new GetAllTypesQuery();
        var types = await typeQueryService.Handle(getAllTypesQuery);
        var enumerable = types as ProductType[] ?? types.ToArray();
        if (enumerable.Length == 0) return NotFound("No product types info found.");
        var typeResources = enumerable.Select(TypeResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(typeResources);
    }
}