using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Controllers;

/// <summary>
/// Controller for account-specific catalog operations.
/// </summary>
[ApiController]
[Route("api/v1/accounts/{accountId}/catalogs")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
public class AccountCatalogController(
    ICatalogCommandService catalogCommandService,
    ICatalogQueryService catalogQueryService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new catalog for an account.",
        Description = "Creates a new catalog for the specified account.",
        OperationId = "CreateCatalogForAccount")]
    [SwaggerResponse(StatusCodes.Status201Created, "Catalog created successfully.", typeof(CatalogResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or failed creation.")]
    public async Task<IActionResult> CreateCatalog(string accountId, [FromBody] CreateCatalogForAccountResource resource)
    {
        try
        {
            // Create the original resource format that the assembler expects
            var originalResource = new CreateCatalogResource(
                resource.name, 
                resource.description, 
                accountId,  // ownerAccount
                resource.contactEmail
            );
            
            var command = CreateCatalogCommandFromResourceAssembler.ToCommandFromResource(originalResource);
            var catalogId = await catalogCommandService.Handle(command);

            var query = new GetCatalogByIdQuery(catalogId.GetId());
            var catalog = await catalogQueryService.Handle(query);

            if (catalog == null)
                return BadRequest("Failed to create catalog.");

            var catalogResource = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalog);
            return StatusCode(StatusCodes.Status201Created, catalogResource);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all catalogs by account.",
        Description = "Retrieves all catalogs owned by the specified account.",
        OperationId = "GetCatalogsByAccount")]
    [SwaggerResponse(StatusCodes.Status200OK, "Catalogs retrieved successfully.", typeof(IEnumerable<CatalogResource>))]
    public async Task<IActionResult> GetCatalogsByAccount(string accountId)
    {
        var query = new GetCatalogsByOwnerQuery(accountId);
        var catalogs = await catalogQueryService.Handle(query);
        var resources = CatalogResourceFromEntityAssembler.ToResourceFromEntity(catalogs);
        return Ok(resources);
    }
}