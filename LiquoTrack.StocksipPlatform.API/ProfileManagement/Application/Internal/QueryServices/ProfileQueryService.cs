using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Application.QueryServices;

/// <summary>
/// Service implementation for handling profile queries.
/// </summary>
public class ProfileQueryService(IProfileRepository profileRepository) : IProfileQueryService
{
    /// <summary>
    /// Method to handle getting a profile by its ID.
    /// </summary>
    /// <param name="query">
    /// The query containing the profile ID.
    /// </param>
    /// <returns>
    /// The profile if found; otherwise, null.
    /// </returns>
    public async Task<Profile?> Handle(GetProfileByIdQuery query)
    {
        return await profileRepository.FindByIdAsync(query.ProfileId);
    }

    /// <summary>
    /// Method to handle getting a profile by user ID.
    /// </summary>
    /// <param name="query">
    /// The query containing the user ID.
    /// </param>
    /// <returns>
    /// The profile if found; otherwise, null.
    /// </returns>
    public async Task<Profile?> Handle(GetProfileByUserIdQuery query)
    {
        return await profileRepository.FindByUserIdAsync(query.UserId);
    }

    /// <summary>
    /// Method to handle getting all profiles.
    /// </summary>
    /// <param name="query">
    /// The query to get all profiles.
    /// </param>
    /// <returns>
    /// A collection of all profiles.
    /// </returns>
    public async Task<IEnumerable<Profile>> Handle(GetAllProfilesQuery query)
    {
        return await profileRepository.FindAllAsync();
    }

    /// <summary>
    /// Method to handle getting profiles by full name.
    /// </summary>
    /// <param name="query">
    /// The query containing the full name to search.
    /// </param>
    /// <returns>
    /// A collection of profiles matching the full name.
    /// </returns>
    public async Task<IEnumerable<Profile>> Handle(GetProfilesByFullNameQuery query)
    {
        return await profileRepository.FindByFullNameAsync(query.FullName);
    }

    /// <summary>
    ///     Method to handle getting profiles by user ID.
    /// </summary>
    /// <param name="query">
    ///     The query containing the user ID.
    /// </param>
    /// <returns>
    ///     A collection of profiles for the specified user.
    /// </returns>
    public async Task<IEnumerable<Profile>> Handle(GetProfilesByUserIdQuery query)
    {
        return await profileRepository.FindAllByUserIdAsync(query.UserId);
    }
}