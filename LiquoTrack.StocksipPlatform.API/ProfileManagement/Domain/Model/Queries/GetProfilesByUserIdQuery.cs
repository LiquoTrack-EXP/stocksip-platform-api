namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Queries;

/// <summary>
///     Method to get all profiles by user ID.
/// </summary>
/// <param name="UserId"></param>
public record GetProfilesByUserIdQuery(string UserId);