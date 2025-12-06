using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Controllers;

/// <summary>
/// REST API controller for managing profiles.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileCommandService _profileCommandService;
    private readonly IProfileQueryService _profileQueryService;
    private readonly ILogger<ProfilesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfilesController"/> class.
    /// </summary>
    /// <param name="profileCommandService">The profile command service.</param>
    /// <param name="profileQueryService">The profile query service.</param>
    /// <param name="logger">The logger instance.</param>
    public ProfilesController(
        IProfileCommandService profileCommandService,
        IProfileQueryService profileQueryService,
        ILogger<ProfilesController> logger)
    {
        _profileCommandService = profileCommandService;
        _profileQueryService = profileQueryService;
        _logger = logger;
    }

    /// <summary>
    ///     Method to get the profile of the currently authenticated user.
    /// </summary>
    /// <returns>
    ///     A <see cref="ProfileResource"/> containing the profile information.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     An argument exception is thrown if the user ID is null or whitespace.
    /// </exception>
    [HttpGet("me")]
    [SwaggerOperation(
        Summary = "Get My Profile",
        Description = "Retrieves the profile of the currently authenticated user.",
        OperationId = "GetMyProfile")]
    [ProducesResponseType(typeof(ProfileResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile()
    {
        var user = HttpContext.Items["User"];
        if (user is null) return Unauthorized("User not authenticated or invalid token.");
        
        
        var userIdProperty = user.GetType().GetProperty("Id") ?? user.GetType().GetProperty("UserId");
        var userId = userIdProperty?.GetValue(user)?.ToString();

        if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or whitespace.", nameof(userId));

        var query = new GetProfileByUserIdQuery(userId);

        var profile = await _profileQueryService.Handle(query);
        if (profile is null) return NotFound($"Profile for user ID {userId} not found.");

        var resource = ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile);

        return Ok(resource);
    }

    /// <summary>
    /// Creates a new profile.
    /// </summary>
    /// <param name="resource">The profile creation data.</param>
    /// <returns>The created profile resource.</returns>
    /// <response code="201">Returns the newly created profile.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost]
    [SwaggerOperation(
        summary: "Create Profile",
        description: "Creates a new profile for the currently authenticated user.")]
    [ProducesResponseType(typeof(ProfileResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProfile([FromForm] CreateProfileResource resource)
    {
        try
        {
            _logger.LogInformation("Creating new profile for UserId: {UserId}", resource.UserId);

            var command = CreateProfileCommandFromResourceAssembler.ToCommandFromResource(resource);
            var profile = await _profileCommandService.Handle(command);

            if (profile == null)
            {
                _logger.LogWarning("Failed to create profile for UserId: {UserId}", resource.UserId);
                return BadRequest("Failed to create profile.");
            }

            var profileResource = ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile);
            
            return CreatedAtAction(
                nameof(GetProfileById), 
                new { id = profile.Id.ToString() }, 
                profileResource);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating profile");
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while creating profile");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile");
            return StatusCode(500, "An error occurred while creating the profile.");
        }
    }

    /// <summary>
    /// Gets a profile by its ID.
    /// </summary>
    /// <param name="id">The profile ID.</param>
    /// <returns>The profile resource.</returns>
    /// <response code="200">Returns the profile.</response>
    /// <response code="404">If the profile is not found.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        summary: "Get Profile By ID",
        description: "Retrieves a profile by its ID.")]
    [ProducesResponseType(typeof(ProfileResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileById(string id)
    {
        try
        {
            _logger.LogInformation("Getting profile with ID: {ProfileId}", id);

            var query = new GetProfileByIdQuery(id);
            var profile = await _profileQueryService.Handle(query);

            if (profile == null)
            {
                _logger.LogWarning("Profile not found with ID: {ProfileId}", id);
                return NotFound($"Profile with ID {id} not found.");
            }

            var resource = ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile);
            return Ok(resource);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile with ID: {ProfileId}", id);
            return StatusCode(500, "An error occurred while retrieving the profile.");
        }
    }

    /// <summary>
    /// Gets a profile by user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The profile resource.</returns>
    /// <response code="200">Returns the profile.</response>
    /// <response code="404">If the profile is not found.</response>
    [HttpGet]
    [SwaggerOperation(
        summary: "Get All profiles by User ID",
        description: "Retrieves all profiles."
        )]
    [ProducesResponseType(typeof(ProfileResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileByUserId([FromQuery] string userId)
    {
        try
        {
            _logger.LogInformation("Getting profile for UserId: {UserId}", userId);

            var query = new GetProfileByUserIdQuery(userId);
            var profile = await _profileQueryService.Handle(query);

            if (profile == null)
            {
                _logger.LogWarning("Profile not found for UserId: {UserId}", userId);
                return NotFound($"Profile for user ID {userId} not found.");
            }

            var resource = ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile);
            return Ok(resource);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile for UserId: {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving the profile.");
        }
    }

    /// <summary>
    /// Updates a profile's information.
    /// </summary>
    /// <param name="id">The profile ID.</param>
    /// <param name="resource">The updated profile data.</param>
    /// <returns>The updated profile resource.</returns>
    /// <response code="200">Returns the updated profile.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the profile is not found.</response>
    [HttpPut("{id}")]
    [SwaggerOperation(
        summary: "Update Profile",
        description: "Updates a profile's information.")]
    [ProducesResponseType(typeof(ProfileResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(string id, [FromForm] UpdateProfileResource resource)
    {
        try
        {
            _logger.LogInformation("Updating profile with ID: {ProfileId}", id);

            var command = UpdateProfileCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var profile = await _profileCommandService.Handle(command);

            if (profile == null)
            {
                _logger.LogWarning("Profile not found with ID: {ProfileId}", id);
                return NotFound($"Profile with ID {id} not found.");
            }

            var profileResource = ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile);
            return Ok(profileResource);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while updating profile");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile with ID: {ProfileId}", id);
            return StatusCode(500, "An error occurred while updating the profile.");
        }
    }

    /// <summary>
    /// Deletes a profile.
    /// </summary>
    /// <param name="id">The profile ID.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the profile was deleted successfully.</response>
    /// <response code="404">If the profile is not found.</response>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        summary: "Delete Profile",
        description: "Deletes a profile.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfile(string id)
    {
        try
        {
            _logger.LogInformation("Deleting profile with ID: {ProfileId}", id);

            var command = DeleteProfileCommandFromResourceAssembler.ToCommandFromResource(id);

            var result = await _profileCommandService.Handle(command);

            if (!result)
            {
                _logger.LogWarning("Profile not found with ID: {ProfileId}", id);
                return NotFound($"Profile with ID {id} not found.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting profile with ID: {ProfileId}", id);
            return StatusCode(500, "An error occurred while deleting the profile.");
        }
    }
}