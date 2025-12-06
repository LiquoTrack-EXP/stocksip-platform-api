using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.Dtos;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services
{
    public interface IUserQueryService
    {
        /// <summary>
        ///     Method to handle the GetAllUsersByAccountIdQuery
        /// </summary>
        /// <param name="query">
        ///     The query object containing the account ID for which users are to be retrieved.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        /// </returns>
        Task<User?> Handle(GetAllUsersByAccountIdQuery query);
        
        /// <summary>
        ///     Method to handle the GetUserByEmailQuery
        /// </summary>
        /// <param name="email">
        ///     The email address of the user.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the user entity if found; otherwise, null.
        /// </returns>
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByEmailAsync(GetUserByEmailQuery query, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of all users.</returns>
        Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Method to handle the GetUserByIdQuery
        /// </summary>
        /// <param name="query">
        ///     The query object containing the user ID for which the user is to be retrieved.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        /// </returns>
        Task<User?> Handle(GetUserByIdQuery query);

        /// <summary>
        ///     Method to handle the GetAccountSubUsersByRoleQuery
        /// </summary>
        /// <param name="query">
        ///     The query object containing the account ID and role for which sub-users are to be retrieved.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        /// </returns>
        Task<UsersWithStatsDto> Handle(GetAccountSubUsersByRoleQuery query);
    }
}
