using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Services;

/// <summary>
/// Service interface for handling profile queries.
/// </summary>
public interface IProfileQueryService
{
    /// <summary>
    /// Handles getting a profile by its ID.
    /// </summary>
    /// <param name="query">The query containing the profile ID.</param>
    /// <returns>The profile if found; otherwise, null.</returns>
    Task<Profile?> Handle(GetProfileByIdQuery query);

    /// <summary>
    /// Handles getting a profile by user ID.
    /// </summary>
    /// <param name="query">The query containing the user ID.</param>
    /// <returns>The profile if found; otherwise, null.</returns>
    Task<Profile?> Handle(GetProfileByUserIdQuery query);

    /// <summary>
    /// Handles getting all profiles.
    /// </summary>
    /// <param name="query">The query to get all profiles.</param>
    /// <returns>A collection of all profiles.</returns>
    Task<IEnumerable<Profile>> Handle(GetAllProfilesQuery query);

    /// <summary>
    /// Handles getting profiles by full name.
    /// </summary>
    /// <param name="query">The query containing the full name to search.</param>
    /// <returns>A collection of profiles matching the full name.</returns>
    Task<IEnumerable<Profile>> Handle(GetProfilesByFullNameQuery query);

    /// <summary>
    ///     Handles getting profiles by user ID.
    /// </summary>
    /// <param name="query">
    ///     The query containing the user ID.
    /// </param>
    /// <returns>
    ///     A collection of profiles for the specified user.
    /// </returns>
    Task<IEnumerable<Profile>> Handle(GetProfilesByUserIdQuery query);
}