using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.Internal.ACL;

/// <summary>
/// Facade for handling profile-related operations, exposed to other bounded contexts.
/// </summary>
public class ProfileContextFacade : IProfileContextFacade
{
    private readonly IProfileCommandService _profileCommandService;
    private readonly IProfileQueryService _profileQueryService;
    private readonly ILogger<ProfileContextFacade> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileContextFacade"/> class.
    /// </summary>
    /// <param name="profileCommandService">The profile command service.</param>
    /// <param name="profileQueryService">The profile query service.</param>
    /// <param name="logger">The logger instance.</param>
    public ProfileContextFacade(
        IProfileCommandService profileCommandService,
        IProfileQueryService profileQueryService,
        ILogger<ProfileContextFacade> logger)
    {
        _profileCommandService =
            profileCommandService ?? throw new ArgumentNullException(nameof(profileCommandService));
        _profileQueryService = profileQueryService ?? throw new ArgumentNullException(nameof(profileQueryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Method to create a new profile.   
    /// </summary>
    /// <param name="userId">
    ///     The ID of the user to create a profile for.
    /// </param>
    /// <param name="firstName">
    ///     The first name of the user.
    /// </param>
    /// <param name="lastName">
    ///     The last name of the user.
    /// </param>
    /// <param name="phoneNumber">
    ///     The phone number of the user.
    /// </param>
    /// <param name="profilePicture">
    ///     The profile picture of the user.
    /// </param>
    /// <param name="assignedRole">
    ///     The role assigned to the user.   
    /// </param>
    /// <returns>
    ///     A string containing the ID of the newly created profile.  
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     A null or empty user ID was provided. 
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     An error occurred while creating the profile.
    /// </exception>
    public async Task<string> CreateProfileAsync(
        string userId,
        string firstName,
        string? lastName,
        string? phoneNumber,
        IFormFile? profilePicture,
        string assignedRole)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

        try
        {
            _logger.LogInformation("Initiating profile creation for user ID: {UserId}", userId);

            var command = new CreateProfileCommand(
                new PersonName(firstName, lastName),
                new PersonContactNumber(phoneNumber),
                phoneNumber,
                profilePicture,
                userId,
                assignedRole
            );

            var profile = await _profileCommandService.Handle(command);

            if (profile == null)
            {
                _logger.LogError("Profile creation failed for user ID: {UserId}", userId);
                throw new InvalidOperationException("Profile creation failed.");
            }

            _logger.LogInformation("Successfully created profile with ID: {ProfileId}", profile.Id);
            return profile.Id.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile for user ID: {UserId}", userId);
            throw new InvalidOperationException("An error occurred while creating the profile.", ex);
        }
    }

    /// <summary>
    ///     Method to get the profile of the currently authenticated user.
    /// </summary>
    /// <param name="userId">
    ///     The ID of the user to find a profile for.
    /// </param>
    /// <returns>
    ///     A profile if found; otherwise, null.   
    /// </returns>
    public async Task<IEnumerable<Profile?>> GetProfilesByUserId(string userId)
    {
        var getProfilesByUserIdQuery = new GetProfilesByUserIdQuery(userId);
        return await _profileQueryService.Handle(getProfilesByUserIdQuery);
    }

    /// <summary>
    ///     Method to delete a profile by ID. 
    /// </summary>
    /// <param name="id">
    ///     The ID of the profile to delete.
    /// </param>
    /// <returns>
    ///     A boolean indicating whether the profile was deleted successfully.
    /// </returns>
    public async Task<bool?> DeleteProfileById(string id)
    {
        var deleteProfileByIdCommand = new DeleteProfileByIdCommand(id);
        return await _profileCommandService.Handle(deleteProfileByIdCommand);
    }
}