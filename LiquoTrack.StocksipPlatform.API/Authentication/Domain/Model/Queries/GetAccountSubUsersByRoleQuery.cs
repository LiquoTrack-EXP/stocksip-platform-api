namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;

/// <summary>
///     Query to get sub-users by role within a specific account.
/// </summary>
/// <param name="AccountId">
///     The unique identifier of the account.
/// </param>
/// <param name="Role">
///     The role of the sub-users to be retrieved.
/// </param>
public record GetAccountSubUsersByRoleQuery(string AccountId, string Role);