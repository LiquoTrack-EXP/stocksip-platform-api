using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;

/// <summary>
///     Interface for User repository operations
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    ///     Finds a user by their email asynchronously
    /// </summary>
    /// <param name="email">The email to search for</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, or null.</returns>
    Task<User?> FindByEmailAsync(string email);

    /// <summary>
    ///     Finds a user by their username asynchronously
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, or null.</returns>
    Task<User?> FindByUsernameAsync(string username);

    /// <summary>
    ///     Finds a user by either email or username asynchronously
    /// </summary>
    /// <param name="email">The email to search for</param>
    /// <param name="username">The username to search for</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found, or null.</returns>
    Task<User?> FindByEmailOrUsernameAsync(string email, string username);

    /// <summary>
    ///     Retrieves all users from the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of all user entities.</returns>
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if a user with the specified username exists in the database.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    bool ExistsByUsername(string username);
    
    /// <summary>
    ///     Method to get all users by account ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account to find users for.   
    /// </param>
    /// <returns>
    ///     A list of users for the specified account.
    /// </returns>
    Task<IEnumerable<User?>> GetUsersByAccountIdAsync(string accountId);
    
    /// <summary>
    ///     Method to count the number of users associated with a specific account ID.
    /// </summary>
    /// <param name="accountId">
    ///     The unique identifier of the account.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing the count of users.   
    /// </returns>
    Task<int> CountByAccountIdAsync(AccountId accountId);
    
}