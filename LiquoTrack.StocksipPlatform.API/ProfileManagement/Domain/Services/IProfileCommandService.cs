using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;

/// <summary>
/// Service interface for handling profile commands.
/// </summary>
public interface IProfileCommandService
{
    /// <summary>
    /// Handles the registration of a new profile.
    /// </summary>
    /// <param name="command">The command containing profile registration data.</param>
    /// <returns>The created profile.</returns>
    Task<Profile?> Handle(CreateProfileCommand command);

    /// <summary>
    /// Handles updating profile information.
    /// </summary>
    /// <param name="command">The command containing updated profile data.</param>
    /// <returns>The updated profile.</returns>
    Task<Profile?> Handle(UpdateProfileCommand command);

    /// <summary>
    /// Handles deleting a profile by its ID.
    /// </summary>
    /// <param name="profileIdCommand">The ID of the profile to delete.</param>
    /// <returns>True if the profile was deleted; otherwise, false.</returns>
    Task<bool> Handle(DeleteProfileByIdCommand profileIdCommand);
}