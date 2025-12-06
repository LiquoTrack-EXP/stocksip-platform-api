using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;

/**
 * <summary>
 *     The user aggregate
 * </summary>
 * <remarks>
 *     This class is used to represent a user
 * </remarks>
 */
public partial class User : Entity
{
    /// <summary>
    ///     The user email.
    /// </summary>
    public Email Email { get; set; }
    
    /// <summary>
    ///     The user username.
    /// </summary>
    public string Username{ get; set; }
    
    /// <summary>
    ///     The user password.
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    ///     The user role.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public EUserRoles UserRole { get; set; }
    
    /// <summary>
    ///     The account id of the user is associated with.
    /// </summary>
    public AccountId AccountId { get; set; }

    /// <summary>
    ///     Default constructor to create a new user (Admin).
    /// </summary>
    /// <param name="email">
    ///     The email.
    /// </param>
    /// <param name="username">
    ///     The username.
    /// </param>
    /// <param name="hashedPassword">
    ///     The hashed password. 
    /// </param>
    /// <param name="accountId">
    ///     The account id of the user is associated with.
    /// </param>
    /// <param name="userRole">
    ///     The user role.
    /// </param>
    public User(Email email, string username, string hashedPassword, string accountId, string userRole)
    {
        Email = email;
        Username = username;
        Password = hashedPassword;
        AccountId = new AccountId(accountId);
        if (!Enum.TryParse<EUserRoles>(userRole, out var role))
            throw new ArgumentException("Invalid user role.", nameof(userRole));

        UserRole = role;
    }
    
    /**
     * <summary>
     *     Changes the password of the user
     * </summary>
     * <param name="hashedPassword">The hashed password</param>
     * <returns>The user</returns>
     */
    public User ChangePassword(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("The password cannot be empty.", nameof(hashedPassword));

        if (hashedPassword.Length < 8)
            throw new ArgumentException("The password must be at least 8 characters long.", nameof(hashedPassword));

        Password = hashedPassword;
        return this;
    }

    /**
     * <summary>
     *     Updates the username of the user
     * </summary>
     * <param name="newUsername">The new username</param>
     * <returns>The user</returns>
     */
    public User UpdateUsername(string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            throw new ArgumentException("The username cannot be empty.", nameof(newUsername));

        if (newUsername.Length < 3)
            throw new ArgumentException("The username must be at least 3 characters.", nameof(newUsername));

        Username = newUsername.Trim();
        return this;
    }

    /**
     * <summary>
     *     Changes the role of the user
     * </summary>
     * <param name="newRoleId">The new role id</param>
     */
    public void ChangeRole(string newRoleId)
    {
        if (string.IsNullOrWhiteSpace(newRoleId))
            throw new ArgumentException("RoleId cannot be empty.", nameof(newRoleId));
        if (!Enum.TryParse<EUserRoles>(newRoleId, out var newRole));
    }
}