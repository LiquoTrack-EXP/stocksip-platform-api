using System.Security.Cryptography;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Email;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Hashing;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Token;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.CommandServices
{
    /// <summary>
    /// Command service for handling user-related operations.
    /// </summary>
    /// <remarks>
    /// This service is responsible for creating and updating user entities in the database.
    /// It also handles the creation of authentication tokens for the user.
    /// </remarks>
    public class UserCommandService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IHashingService hashingService,
        IPaymentAndSubscriptionsFacade paymentAndSubscriptionsFacade,
        IProfileContextFacade profileContextFacade,
        IEmailService emailService
    ) : IUserCommandService
    {
        /// <summary>
        /// Handles the creation of a new user.
        /// </summary>
        /// <param name="command">The command containing user details.</param>
        /// <returns>The created user entity.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a user with the same username already exists.</exception>
        public async Task<User?> Handle(CreateUserCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var existingUser = await userRepository.FindByUsernameAsync(command.Username);
            if (existingUser != null)
                throw new InvalidOperationException($"Username {command.Username} is already taken");

            var hashedPassword = hashingService.HashPassword(command.Password);
            var user = new User(
                new Email(command.Email.Value),
                command.Username,
                hashedPassword,
                "1234",
                command.UserRole
            );

            try
            {
                await userRepository.AddAsync(user);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating user: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates or updates a user from an external authentication provider.
        /// </summary>
        /// <param name="providerUserId">The unique identifier of the user from the external provider.</param>
        /// <param name="email">The email address of the user.</param>
        /// <param name="name">The name of the user (optional).</param>
        /// <param name="accountId">The account ID of the user (optional).</param>
        /// <returns>The created or updated user entity.</returns>
        /// <exception cref="ArgumentException">Thrown when the email is null or whitespace.</exception>
        public async Task<User> CreateOrUpdateFromExternalAsync(string providerUserId, string email,
            string? name = null, string? accountId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            var existingUser = await userRepository.FindByUsernameAsync(email);

            if (existingUser != null)
            {
                if (existingUser.AccountId is null ||
                    string.IsNullOrWhiteSpace(existingUser.AccountId.GetId) ||
                    existingUser.AccountId.GetId == "1234")
                {
                    var resolvedAccountId = await EnsureAccountStructureAsync(name, email);
                    existingUser.AccountId = new AccountId(resolvedAccountId);
                    await userRepository.UpdateAsync(existingUser);
                }

                return existingUser;
            }

            var randomPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            var hashedPassword = hashingService.HashPassword(randomPassword);
            var accountIdForNewUser = await EnsureAccountStructureAsync(name, email);

            var displayName = string.IsNullOrWhiteSpace(name) ? email.Split('@')[0] : name;
            var user = new User(
                new Email(email),
                displayName,
                hashedPassword,
                accountIdForNewUser,
                EUserRoles.SuperAdmin.ToString()
            );

            try
            {
                await profileContextFacade.CreateProfileAsync(
                    userId: user.Id.ToString(),
                    firstName: user.Username,
                    lastName: "",
                    phoneNumber: "+10000000000",
                    profilePicture: null,
                    assignedRole: "Admin"
                );

                await userRepository.AddAsync(user);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating user from external provider: {ex.Message}", ex);
            }

            async Task<string> EnsureAccountStructureAsync(string? fullName, string mail)
            {
                var businessName = string.IsNullOrWhiteSpace(fullName)
                    ? mail.Split('@')[0]
                    : fullName.Trim();

                var business = await paymentAndSubscriptionsFacade.CreateBusiness(businessName);
                if (business is null)
                    throw new Exception("Business creation failed for external authentication user");

                var account = await paymentAndSubscriptionsFacade.CreateAccount(
                    EAccountRole.LiquorStoreOwner.ToString(),
                    business.Id.ToString());

                if (account is null)
                    throw new Exception("Account creation failed for external authentication user");

                return account.Id.ToString();
            }
        }

        /// <summary>
        ///     Handles the sign in command
        /// </summary>
        /// <param name="command">The sign in command</param>
        /// <returns>The user entity</returns>
        public async Task<(User user, string token)> Handle(SignInCommand command)
        {
            var user = await userRepository.FindByEmailAsync(command.Email);

            if (user == null || !hashingService.VerifyPassword(command.Password, user.Password))
                throw new Exception("Invalid username or password");

            var token = tokenService.GenerateToken(user);
            
            return (user, token);
        }

        /// <summary>
        ///     Handles the sign-up command
        /// </summary>
        /// <param name="command">The sign-up command</param>
        /// <returns>The user entity</returns>
        public async Task<User?> Handle(SignUpCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            try
            {
                var business = await paymentAndSubscriptionsFacade.CreateBusiness(
                    businessName: command.BusinessName
                );
                
                if (business == null) throw new Exception("Business creation failed");

                var account = await paymentAndSubscriptionsFacade.CreateAccount(
                    role: command.Role,
                    businessId: business.Id.ToString()
                );

                if (account == null) throw new Exception("Account creation failed");
                
                var existingUser = await userRepository.FindByEmailAsync(command.Email);
                if (existingUser != null)
                    throw new InvalidOperationException($"Email {command.Email} is already registered");

                var hashedPassword = hashingService.HashPassword(command.Password);
                var user = new User(
                    new Email(command.Email),
                    command.Name,
                    hashedPassword,
                    account.Id.ToString(),
                    "SuperAdmin"
                );
                
                await profileContextFacade.CreateProfileAsync(
                    userId: user.Id.ToString(),
                    firstName: user.Username,
                    lastName: "",
                    phoneNumber: "+10000000000",
                    profilePicture: null,
                    assignedRole: "Admin"
                );
                
                await userRepository.AddAsync(user);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"SignUp failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Method to register a sub user.
        /// </summary>
        /// <param name="command">
        ///     The command containing the details for registering a sub user.
        /// </param>
        /// <returns>
        ///     A user object representing the newly registered sub user.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     A null command is provided.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     A user with the same email address already exists.
        /// </exception>
        /// <exception cref="Exception">
        ///     An error occurred while registering the sub user.
        /// </exception>
        public async Task<User?> Handle(RegisterSubUserCommand command)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            var existingUser = await userRepository.FindByEmailAsync(command.Email);
            if (existingUser != null) throw new InvalidOperationException($"Email {command.Email} is already registered");

            var currentUserCount = await userRepository.CountByAccountIdAsync(new AccountId(command.AccountId));
            var maxAllowedUsers = await paymentAndSubscriptionsFacade.GetPlanUserLimitByAccountId(command.AccountId);
            
            if (maxAllowedUsers is not null && currentUserCount >= maxAllowedUsers)
                throw new InvalidOperationException("The account has reached the maximum number of users for the current plan.");

            var hashedPassword = hashingService.HashPassword(command.Password);
            
            var user = new User(
                new Email(command.Email),
                command.Name,
                hashedPassword,
                command.AccountId,
                command.Role
            );

            try
            {
                await profileContextFacade.CreateProfileAsync(
                    userId: user.Id.ToString(),
                    firstName: user.Username,
                    lastName: "",
                    phoneNumber: command.PhoneNumber,
                    profilePicture: null,
                    assignedRole: command.ProfileRole
                );
                
                await userRepository.AddAsync(user);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while registering sub user: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Method to delete a sub user.
        /// </summary>
        /// <param name="command">
        ///     The command containing the details for deleting a sub user.
        /// </param>
        /// <returns>
        ///     A boolean indicating whether the sub user was deleted successfully.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     An error occurred while deleting the sub user.
        /// </exception>
        public async Task<bool?> Handle(DeleteUserWithProfielByIdCommand command)
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                throw new InvalidOperationException($"User with ID {command.UserId} does not exist.");

            try
            {
                await userRepository.DeleteAsync(command.UserId);
                await profileContextFacade.DeleteProfileById(command.ProfileId);
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"An error occurred while deleting sub user with ID: {command.UserId}. Details: {ex.Message}", ex);   
            }

        }

        /// <summary>
        ///     Method to send a recovery code to the user's email address.'
        /// </summary>
        /// <param name="command">
        ///     Command containing the details for sending a recovery code.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     An error occurred while sending the recovery code.
        /// </exception>
        public async Task Handle(SendCodeToRecoverPasswordCommand command)
        {
            var user = await userRepository.FindByEmailAsync(command.Email);
            if (user is null)
                throw new InvalidOperationException($"User with email {command.Email} does not exist.");
            
            var code = new Random().Next(100000, 999999).ToString();
            var hashedCode = hashingService.HashPassword(code);
            user.SetRecoveryCode(hashedCode, TimeSpan.FromMinutes(15));

            try
            {
                await emailService.SendEmail(user.Email.Value, "Use this to recover your password",
                    $"Your password recovery code is: {code}. It is valid for 15 minutes.");
                await userRepository.UpdateAsync(user);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(
                    $"An error occurred while sending recovery code to email {command.Email}. Details: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Method to verify a recovery code.
        /// </summary>
        /// <param name="command">
        ///     The command containing the details for verifying a recovery code.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     An error occurred while verifying the recovery code.
        /// </exception>
        public async Task Handle(VerifyRecoveryCodeCommand command)
        {
            var user = await userRepository.FindByEmailAsync(command.Email);
            if (user is null)
                throw new InvalidOperationException($"User with email {command.Email} does not exist.");
            
            if (!hashingService.VerifyPassword(command.RecoveryCode, user.RecoveryCode))
                throw new Exception("Invalid username or password");

            if (!user.IsRecoveryCodeExpirationTimeValid)
                throw new InvalidOperationException("The recovery code has expired.");
            
            user.ClearRecoveryCode();
            await userRepository.UpdateAsync(user);
        }

        /// <summary>
        ///     Method to reset a user's password.'
        /// </summary>
        /// <param name="command">
        ///     The command containing the details for resetting a user's password.'
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     An error occurred while resetting the user's password.'
        /// </exception>
        public async Task Handle(ResetPasswordCommand command)
        {
            var user = await userRepository.FindByEmailAsync(command.Email);
            if (user is null)
                throw new InvalidOperationException($"User with email {command.Email} does not exist.");

            var hashPassword = hashingService.HashPassword(command.NewPassword);
            user.UpdatePassword(hashPassword);
            await userRepository.UpdateAsync(user);
        }
    }
}