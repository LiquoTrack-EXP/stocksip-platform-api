using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.Dtos;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;

/// <summary>
///     Assembler to transform DTOs from the application layer into REST resources.
/// </summary>
public static class UserWithProfileResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a single user DTO into a REST resource.
    /// </summary>
    public static UsersWithProfilesResource ToResourceFromEntity(UsersWithProfilesDto dto)
    {
        return new UsersWithProfilesResource(
            dto.UserId,
            dto.Email,
            dto.Role,
            dto.ProfileId,
            dto.FullName,
            dto.PhoneNumber,
            dto.ProfilePictureUrl,
            dto.profileRole
        );
    }

    /// <summary>
    ///     Converts a list of user DTOs into REST resources.
    /// </summary>
    public static IEnumerable<UsersWithProfilesResource> ToResourceListFromEntityList(IEnumerable<UsersWithProfilesDto> dtos)
    {
        return dtos.Select(ToResourceFromEntity);
    }

    /// <summary>
    ///     Converts a users-with-stats DTO into a REST resource.
    /// </summary>
    public static UsersWithStatsResource ToResourceFromEntity(UsersWithStatsDto dto)
    {
        return new UsersWithStatsResource(
            dto.MaxUsersAllowed,
            dto.TotalUsers,
            ToResourceListFromEntityList(dto.Users)
        );
    }
}