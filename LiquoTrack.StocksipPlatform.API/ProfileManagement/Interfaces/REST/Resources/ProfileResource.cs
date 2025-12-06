namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Resources;

/// <summary>
/// Resource representing a profile.
/// </summary>
/// <param name="Id">The profile ID.</param>
/// <param name="FirstName">The first name of the person.</param>
/// <param name="LastName">The last name of the person.</param>
/// <param name="FullName">The full name of the person.</param>
/// <param name="PhoneNumber">The phone number.</param>
/// <param name="ContactNumber">The contact number as string.</param>
/// <param name="ProfilePictureUrl">The profile picture URL.</param>
/// <param name="UserId">The user ID associated with the profile.</param>
/// <param name="AssignedRole">The assigned role in the profile.</param>
public record ProfileResource(
    string Id,
    string FirstName,
    string LastName,
    string FullName,
    string PhoneNumber,
    string ContactNumber,
    string? ProfilePictureUrl,
    string UserId,
    string AssignedRole
);