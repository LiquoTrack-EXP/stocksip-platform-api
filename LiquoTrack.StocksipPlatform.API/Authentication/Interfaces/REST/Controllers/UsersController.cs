using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Attributes;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling user-related requests.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for users.")]
public class UsersController(
    IUserCommandService userCommandService,
    IUserQueryService userQueryService) : ControllerBase
{
    /**
     * <summary>
     *     Get user by id endpoint. It allows getting a user by id
     * </summary>
     * <param name="userId">The user id</param>
     * <returns>The user resource</returns>
     */
    [HttpGet("{userId}")]
    [SwaggerOperation(
        Summary = "Get a user by its id",
        Description = "Get a user by its id",
        OperationId = "GetUserById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was found", typeof(UserResource))]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var getUserByIdQuery = new GetUserByIdQuery(userId);
        var user = await userQueryService.Handle(getUserByIdQuery);
        if (user == null) return NotFound($"User with {userId} not found");
        var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
        return Ok(userResource);
    }

    /// <summary>
    ///     Method to delete a user with profile by its id.
    /// </summary>
    /// <param name="userId">
    ///     The ID of the user to delete.
    /// </param>
    /// <param name="resource">
    ///     The resource containing the ID of the profile to delete.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the details of the deleted user, or a 404 Not Found response if the user could not be found. 
    /// </returns>
    [HttpDelete("{userId}")]
    [SwaggerOperation(
        Summary = "Delete a user with profile by its id",
        Description = "Delete a user with profile by its id"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The user was not found.")]
    public async Task<IActionResult> DeleteUserWithProfileById(string userId, [FromBody] DeleteUserWithProfileResource resource)
    {
        var command =
            DeleteUserWithProfileCommandFromResourceAssembler.ToCommandFromResource(userId, resource.ProfileId);
        await userCommandService.Handle(command);
        return Ok(new { Message = $"User with Id {userId} and profile Id {resource.ProfileId} deleted successfully." });
    }
}