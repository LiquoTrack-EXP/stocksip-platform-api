namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Commands;

/// <summary>
///     Command to delete a profile by id.
/// </summary>
/// <param name="ProfileId">
///     The unique identifier of the profile.
/// </param>
public record DeleteProfileByIdCommand(string ProfileId);