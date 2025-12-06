using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Assemblers;

public static class ProfileResourceFromEntityAssembler
{
    /// <summary>
    /// Converts a Profile entity to a ProfileResource.
    /// </summary>
    /// <param name="profile">The profile entity.</param>
    /// <returns>The profile resource.</returns>
    public static ProfileResource ToResourceFromEntity(Profile profile)
    {
        return new ProfileResource(
            profile.Id.ToString(),
            profile.Name?.FirstName ?? string.Empty,
            profile.Name?.LastName ?? string.Empty,
            profile.FullName ?? string.Empty,
            profile.PersonContactNumber.PhoneNumber ?? string.Empty,
            profile.ContactNumber ?? string.Empty,
            profile.ProfilePictureUrl?.GetValue(),
            profile.UserId,
            profile.AssignedRole.ToString()
        );
    }
}