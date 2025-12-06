using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Attributes;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling plan-related requests.
/// </summary>
/// <param name="planQueryService">
///     The service for handling plan-related queries.
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for plans.")]
public class PlansController(IPlanQueryService planQueryService) : ControllerBase
{
    
    /// <summary>
    ///     Endpoint to handle the retrieval of all available plans. 
    /// </summary>
    /// <returns>
    ///     The list of all available plans.
    /// </returns>
    [AllowAnonymous]
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all plans",
        Description = "Retrieves a list of all available plans.",
        OperationId = "GetAllPlans")]
    [SwaggerResponse(StatusCodes.Status200OK, "Plans retrieved successfully.", typeof(IEnumerable<PlanResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No available plans found.")]
    public async Task<IActionResult> GetAllPlans()
    {
        var getAllPlansQuery = new GetAllPlansQuery();
        var plans = await planQueryService.Handle(getAllPlansQuery);
        var enumerable = plans as Plan[] ?? plans.ToArray();
        if (enumerable.Length == 0) return NotFound("No available plans found");
        var planResource = enumerable.Select(PlanResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(planResource);
    }
}