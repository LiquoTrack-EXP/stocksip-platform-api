namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

/// <summary>
///     Resource representing a list of users with statistics.
/// </summary>
public record UsersWithStatsResource(int? MaxUsersAllowed, int TotalUsers, IEnumerable<UsersWithProfilesResource> Users);