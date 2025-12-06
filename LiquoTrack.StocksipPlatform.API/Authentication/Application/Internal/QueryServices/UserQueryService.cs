using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.Dtos;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.QueryServices
{
    /// <summary>
    /// Query service for handling user-related operations.
    /// </summary>
    /// <remarks>
    /// This service is responsible for retrieving user entities from the database.
    /// It also handles the validation of authentication tokens for the user.
    /// </remarks>
    public class UserQueryService : IUserQueryService
    {
        private readonly IUserRepository _userRepository;
        
        private readonly IProfileContextFacade _profileContextFacade;
        
        private readonly IPaymentAndSubscriptionsFacade _paymentAndSubscriptionsFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserQueryService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="profileContextFacade">The profile context facade.</param>
        /// <param name="paymentAndSubscriptionsFacade">The payment and subscriptions facade.</param>
        public UserQueryService(IUserRepository userRepository, IProfileContextFacade profileContextFacade, IPaymentAndSubscriptionsFacade paymentAndSubscriptionsFacade)
        {
            _userRepository = userRepository;
            _profileContextFacade = profileContextFacade;
            _paymentAndSubscriptionsFacade = paymentAndSubscriptionsFacade;       
        }

        /// <summary>
        /// Handles the retrieval of a user by their account ID.
        /// </summary>
        /// <param name="query">The query containing the account ID.</param>
        /// <returns>The user entity if found; otherwise, null.</returns>
        public async Task<User?> Handle(GetAllUsersByAccountIdQuery query)
        {
            return await _userRepository.FindByIdAsync(query.AccountId.ToString());
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user entity if found; otherwise, null.</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.FindByEmailAsync(email);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="query">The email address of the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>       
        /// <returns>The user entity if found; otherwise, null.</returns>
        public async Task<IEnumerable<User>> GetUsersByEmailAsync(GetUserByEmailQuery query, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByEmailAsync(query.Email);
            return user != null ? new List<User> { user } : new List<User>();
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of all users.</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetAllUsersAsync(cancellationToken);
        }

        /// <summary>
        ///     Retrieves a user by their ID.
        /// </summary>
        /// <param name="query">
        ///     The query object containing the user ID.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        /// </returns>
        public async Task<User?> Handle(GetUserByIdQuery query)
        {
            return await _userRepository.FindByIdAsync(query.UserId.ToString());
        }

        /// <summary>
        ///     Method to handle the GetAccountSubUsersByRoleQuery
        /// </summary>
        /// <param name="query">
        ///     The query object containing the account ID and role for which sub-users are to be retrieved.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains a list of sub-users.
        /// </returns>
        public async Task<UsersWithStatsDto> Handle(GetAccountSubUsersByRoleQuery query)
        {
            var users = await _userRepository.GetUsersByAccountIdAsync(query.AccountId);

            var result = new List<UsersWithProfilesDto>();
            
            var currentUserCount = await _userRepository.CountByAccountIdAsync(new AccountId(query.AccountId));
            var maxAllowedUsers = await _paymentAndSubscriptionsFacade.GetPlanUserLimitByAccountId(query.AccountId);
            
            var filteredUsers = query.Role.Equals("All", StringComparison.OrdinalIgnoreCase)
                ? users
                : users.Where(u => u != null && 
                                   u.UserRole.ToString()
                                       .Equals(query.Role, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            foreach (var user in filteredUsers)
            {
                if (user == null) continue;

                var profiles = await _profileContextFacade.GetProfilesByUserId(user.Id.ToString());
                var profile = profiles.FirstOrDefault();

                result.Add(new UsersWithProfilesDto(
                    user.Id.ToString(),
                    user.Email.GetValue,
                    user.UserRole.ToString(),
                    profile?.Id.ToString(),
                    profile?.FullName,
                    profile?.ContactNumber,
                    profile?.ProfilePictureUrl.GetValue(),
                    profile?.AssignedRole.ToString()
                ));
            }

            return new UsersWithStatsDto(
                MaxUsersAllowed: maxAllowedUsers,
                TotalUsers: currentUserCount,
                Users: result
            );
        }
    }
}