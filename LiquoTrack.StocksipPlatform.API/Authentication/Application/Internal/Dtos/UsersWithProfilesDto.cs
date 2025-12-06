namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.Dtos;

/// <summary>
///     Dto for user with profile information.
/// </summary>
public record UsersWithProfilesDto(
    string UserId,
    string Email,
    string Role,
    string? ProfileId,
    string? FullName,
    string? PhoneNumber,
    string? ProfilePictureUrl,
    string? profileRole
);