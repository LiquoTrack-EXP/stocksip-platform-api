using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Repositories;

/// <summary>
/// Repository interface for Profile aggregate.
/// </summary>
public interface IProfileRepository
{
    /// <summary>
    /// Adds a new profile to the repository.
    /// </summary>
    /// <param name="profile">The profile to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Profile profile);

    /// <summary>
    /// Updates an existing profile in the repository.
    /// </summary>
    /// <param name="profile">The profile to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Profile profile);

    /// <summary>
    /// Deletes a profile from the repository.
    /// </summary>
    /// <param name="profile">The profile to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Profile profile);

    /// <summary>
    /// Finds a profile by its ID.
    /// </summary>
    /// <param name="id">The ID of the profile.</param>
    /// <returns>The profile if found; otherwise, null.</returns>
    Task<Profile?> FindByIdAsync(ObjectId id);

    /// <summary>
    /// Finds a profile by its ID as string.
    /// </summary>
    /// <param name="id">The ID of the profile as string.</param>
    /// <returns>The profile if found; otherwise, null.</returns>
    Task<Profile?> FindByIdAsync(string id);

    /// <summary>
    /// Finds a profile by user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The profile if found; otherwise, null.</returns>
    Task<Profile?> FindByUserIdAsync(string userId);

    /// <summary>
    /// Finds profiles by full name.
    /// </summary>
    /// <param name="fullName">The full name to search for.</param>
    /// <returns>A collection of profiles matching the full name.</returns>
    Task<IEnumerable<Profile>> FindByFullNameAsync(string fullName);

    /// <summary>
    /// Gets all profiles.
    /// </summary>
    /// <returns>A collection of all profiles.</returns>
    Task<IEnumerable<Profile>> FindAllAsync();

    /// <summary>
    /// Checks if a profile exists by user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>True if a profile exists; otherwise, false.</returns>
    Task<bool> ExistsByUserIdAsync(string userId);
    
    /// <summary>
    ///     Checks if a profile exists by its ID.
    /// </summary>
    /// <param name="profileId">
    ///     The ID of the profile to check for existence.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    Task<string> FindProfilePictureUrlByIdAsync(ObjectId profileId);

    /// <summary>
    ///     Method to find all users by account ID.
    /// </summary>
    /// <param name="userId">
    ///     The ID of the account to find users for.  
    /// </param>
    /// <returns>
    ///     A list of users for the specified account.
    /// </returns>
    Task<IEnumerable<Profile>> FindAllByUserIdAsync(string userId);
}