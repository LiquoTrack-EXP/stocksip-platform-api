namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Resources;

/// <summary>
/// Resource for updating profile information.
/// </summary>
/// <param name="FirstName">The first name of the person.</param>
/// <param name="LastName">The last name of the person.</param>
/// <param name="PhoneNumber">The phone number.</param>
/// <param name="ProfilePicture">The profile picture.</param>
public record UpdateProfileResource(
    string FirstName,
    string LastName,
    string PhoneNumber,
    IFormFile? ProfilePicture,
    string AssignedRole
);