using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly IMongoCollection<User> _collection;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        public UserRepository(AppDbContext context, IMediator mediator) : base(context, mediator)
        {
            _collection = context.GetCollection<User>();
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user entity if found; otherwise, null.</returns>
        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _collection
                .Find(user => user.Email.ToString() == email)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The user entity if found; otherwise, null.</returns>
        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _collection
                .Find(user => user.Username == username)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a user by their email address or username.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <returns>The user entity if found; otherwise, null.</returns>
        public async Task<User?> FindByEmailOrUsernameAsync(string email, string username)
        {
            return await _collection
                .Find(user => user.Email.ToString() == email || user.Username == username)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of all user entities.</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _collection
                .Find(_ => true)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Checks if a user with the specified username exists in the database.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        public bool ExistsByUsername(string username)
        {
            return _collection
                .Find(user => user.Username == username)
                .Any();
        }

        /// <summary>
        ///     Method to get all users by account id.
        /// </summary>
        /// <param name="accountId">
        ///     The ID of the account to find users for. 
        /// </param>
        /// <returns>
        ///     A list of users for the specified account.
        /// </returns>
        public async Task<IEnumerable<User?>> GetUsersByAccountIdAsync(string accountId)
        {
            return await _collection
                .Find(user => user.AccountId.ToString() == accountId)
                .ToListAsync();
        }

        /// <summary>
        ///     Method to count users by account id.
        /// </summary>
        /// <param name="accountId">
        ///     The ID of the account to count users for.
        /// </param>
        /// <returns>
        ///     A count of users for the specified account.
        /// </returns>
        public async Task<int> CountByAccountIdAsync(AccountId accountId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.AccountId, accountId);
            return (int)await _collection.CountDocumentsAsync(filter);
        }
    }
}