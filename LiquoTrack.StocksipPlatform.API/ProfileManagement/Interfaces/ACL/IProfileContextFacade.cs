using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.ACL;

/// <summary>
/// ACL interface for exposing Profile context operations to other bounded contexts.
/// </summary>
public interface IProfileContextFacade
{
    /// <summary>
    /// Creates a new profile for a given user with basic info.
    /// </summary>
    /// <param name="userId">The user ID to associate with the profile.</param>
    /// <param name="firstName">The first name of the profile owner.</param>
    /// <param name="lastName">The last name of the profile owner.</param>
    /// <param name="phoneNumber">The phone number of the profile owner.</param>
    /// <param name="profilePicture">The profile picture.</param>
    /// <param name="assignedRole">The role assigned to this profile.</param>
    /// <returns>The ID of the newly created profile.</returns>
    Task<string> CreateProfileAsync(
        string userId,
        string firstName,
        string? lastName,
        string? phoneNumber,
        IFormFile? profilePicture,
        string assignedRole);
    
    /// <summary>
    ///     Method to get a profile by user ID.   
    /// </summary>
    /// <param name="userId">
    ///     The ID of the user to find a profile for.
    /// </param>
    /// <returns>
    ///     A profile if found; otherwise, null.
    /// </returns>
    Task<IEnumerable<Profile?>> GetProfilesByUserId(string userId);
    
    /// <summary>
    ///     Method to delete a profile by ID.  
    /// </summary>
    /// <param name="id">
    ///     The ID of the profile to delete. 
    /// </param>
    /// <returns>
    ///     A profile if found; otherwise, null.
    /// </returns>
    Task<bool?> DeleteProfileById(string id);
}