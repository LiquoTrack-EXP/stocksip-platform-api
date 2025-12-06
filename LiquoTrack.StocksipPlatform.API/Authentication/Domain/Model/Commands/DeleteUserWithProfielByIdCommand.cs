namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;

/// <summary>
///     Command to delete a sub user by Id
/// </summary>
/// <param name="UserId">
///     The unique identifier of the user.
/// </param>
/// <param name="ProfileId">
///     The unique identifier of the profile.
/// </param>
public record DeleteUserWithProfielByIdCommand(string UserId, string ProfileId);