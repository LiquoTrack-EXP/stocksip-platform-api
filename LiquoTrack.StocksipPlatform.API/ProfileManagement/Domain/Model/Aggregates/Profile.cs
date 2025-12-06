using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;

/// <summary>
/// The Profile Aggregate Root entity.
/// </summary>
public class Profile : Entity
{
    /// <summary>
    /// Gets the name of the person.
    /// </summary>
    public PersonName Name { get; private set; }

    /// <summary>
    /// Gets the contact number of the person.
    /// </summary>
    public PersonContactNumber PersonContactNumber { get; private set; }

    /// <summary>
    /// Gets the contact number as a string.
    /// </summary>
    public string ContactNumber { get; private set; }

    /// <summary>
    /// Gets the full name of the person.
    /// </summary>
    public string FullName { get; private set; }

    /// <summary>
    /// Gets the profile picture URL.
    /// </summary>
    public ImageUrl? ProfilePictureUrl { get; private set; }

    /// <summary>
    /// Gets the user ID associated with this profile.
    /// </summary>
    public string UserId { get; private set; }
    
    /// <summary>
    /// Gets or sets the assigned role for the profile.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public EProfileRole AssignedRole { get; private set; }

    /// <summary>
    /// Primary constructor for the Profile Aggregate Root.
    /// </summary>
    /// <param name="name">The name of the person.</param>
    /// <param name="personContactNumber">The contact number object.</param>
    /// <param name="contactNumber">The contact number as string.</param>
    /// <param name="profilePictureUrl">The profile picture URL.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="assignedRole">The assigned role for the profile.</param>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
    /// <exception cref="ArgumentException">Thrown when userId is null or whitespace.</exception>
    public Profile(
        PersonName name,
        PersonContactNumber personContactNumber,
        string contactNumber,
        ImageUrl? profilePictureUrl,
        string userId,
        string assignedRole)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PersonContactNumber = personContactNumber ?? throw new ArgumentNullException(nameof(personContactNumber));
        ContactNumber = contactNumber ?? throw new ArgumentNullException(nameof(contactNumber));
        ProfilePictureUrl = profilePictureUrl;

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or whitespace.", nameof(userId));

        if (string.IsNullOrWhiteSpace(assignedRole))
            throw new ArgumentException("Assigned role cannot be null or whitespace.", nameof(assignedRole));

        UserId = userId;
        FullName = name.GetFullName();
        AssignedRole = Enum.Parse<EProfileRole>(assignedRole);
    }

    /// <summary>
    /// Command constructor for the Profile Aggregate Root.
    /// </summary>
    /// <param name="command">
    /// The command that triggered the creation of the profile.
    /// It contains the information needed to create the profile.
    /// </param>
    public Profile(CreateProfileCommand command, string imageUrl) : this(
        command.Name,
        command.PersonContactNumber,
        command.ContactNumber,
        new ImageUrl(imageUrl),
        command.UserId,
        command.AssignedRole)
    {
    }

    /// <summary>
    /// Method to update the profile information.
    /// </summary>
    /// <param name="command">
    /// The command containing the new profile information.
    /// </param>
    public void UpdateInformation(UpdateProfileCommand command, string imageUrl)
    {
        Name = command.Name;
        PersonContactNumber = command.PersonContactNumber;
        ContactNumber = command.ContactNumber;
        ProfilePictureUrl = new ImageUrl(imageUrl);
        FullName = command.Name.GetFullName();
        AssignedRole = Enum.Parse<EProfileRole>(command.AssignedRole);
    }
}