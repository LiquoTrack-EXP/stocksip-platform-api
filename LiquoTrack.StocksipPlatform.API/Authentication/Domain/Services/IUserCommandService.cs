using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;

public interface IUserCommandService
{
    /// <summary>
    ///     Method to create a new user.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for creating a new user.
    /// </param>
    /// <returns>
    ///     A user object representing the newly created user.
    /// </returns>
    Task<User?> Handle(CreateUserCommand command);
    
    /// <summary>
    ///     Method to create or update a user from an external provider.
    /// </summary>
    /// <param name="providerUserId">
    ///     The ID of the user from the external provider. 
    /// </param>
    /// <param name="email">
    ///     The email address of the user.
    /// </param>
    /// <param name="name">
    ///     The name of the user.
    /// </param>
    /// <param name="accountId">
    ///     The ID of the account the user belongs to.
    /// </param>
    /// <returns>
    ///     A user object representing the created or updated user.
    /// </returns>
    Task<User> CreateOrUpdateFromExternalAsync(string providerUserId, string email, string? name, string? accountId = null);
    
    /// <summary>
    ///     Method to sign in a user.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for signing in a user.   
    /// </param>
    /// <returns>
    ///     A user object representing the signed in user.
    /// </returns>
    Task<(User user, string token)> Handle(SignInCommand command);
    
    /// <summary>
    ///     Method to register a new user.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for registering a new user.
    /// </param>
    /// <returns>
    ///     A user object representing the newly registered user.
    /// </returns>
    Task<User?> Handle(SignUpCommand command);

    /// <summary>
    ///     Method to register a sub user.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for registering a sub user.
    /// </param>
    /// <returns>
    ///     A user object representing the newly registered sub user.
    /// </returns>
    Task<User?> Handle(RegisterSubUserCommand command);

    /// <summary>
    ///     Method to delete a user.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for deleting a user.
    /// </param>
    /// <returns>
    ///     A boolean indicating whether the user was deleted successfully.
    /// </returns>
    Task<bool?> Handle(DeleteUserWithProfielByIdCommand command);
    
    /// <summary>
    ///     Method to send a recovery code to the user.
    /// </summary>
    /// <param name="command">
    ///     Command containing the details for sending a recovery code.
    /// </param>
    /// <returns>
    ///     A boolean indicating whether the recovery code was sent successfully.
    /// </returns>
    Task Handle(SendCodeToRecoverPasswordCommand command);

    /// <summary>
    ///     Method to verify a recovery code.
    /// </summary>
    /// <param name="command">
    ///     Command containing the details for verifying a recovery code.
    /// </param>
    /// <returns>
    ///     A boolean indicating whether the recovery code was verified successfully.
    /// </returns>
    Task Handle(VerifyRecoveryCodeCommand command);
    
    /// <summary>
    ///     Method to reset a user's password.
    /// </summary>
    /// <param name="command">
    ///     Command containing the details for resetting a user's password.'
    /// </param>
    /// <returns>
    ///     A boolean indicating whether the password was reset successfully.
    /// </returns>
    Task Handle(ResetPasswordCommand command);
}