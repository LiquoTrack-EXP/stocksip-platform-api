using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Repository implementation for the Profile aggregate. 
/// </summary>
public class ProfileRepository(AppDbContext context, IMediator mediator) : BaseRepository<Profile>(context, mediator), IProfileRepository
{
    /// <summary>
    ///     The MongoDB collection for the Profile aggregate.   
    /// </summary>
    private readonly IMongoCollection<Profile> _profileCollection = context.GetCollection<Profile>();

    /// <inheritdoc />
    public new async Task AddAsync(Profile profile)
    {
        await _profileCollection.InsertOneAsync(profile);
    }
    
    /// <inheritdoc />
    public async Task UpdateAsync(Profile profile)
    {
        await _profileCollection.ReplaceOneAsync(
            x => x.Id == profile.Id,
            profile,
            new ReplaceOptions { IsUpsert = false }
        );
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Profile profile)
    {
        await _profileCollection.DeleteOneAsync(x => x.Id == profile.Id);
    }

    /// <inheritdoc />
    public async Task<Profile?> FindByIdAsync(ObjectId id)
    {
        return await _profileCollection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public new async Task<Profile?> FindByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        return await _profileCollection
            .Find(x => x.Id == objectId)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<Profile?> FindByUserIdAsync(string userId)
    {
        return await _profileCollection
            .Find(x => x.UserId == userId)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Profile>> FindByFullNameAsync(string fullName)
    {
        return await _profileCollection
            .Find(x => x.FullName == fullName)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Profile>> FindAllAsync()
    {
        return await _profileCollection
            .Find(FilterDefinition<Profile>.Empty)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByUserIdAsync(string userId)
    {
        return await _profileCollection
            .Find(x => x.UserId == userId)
            .AnyAsync();
    }
    
    /// <summary>
    ///     Method to find the profile picture URL by profile ID. 
    /// </summary>
    /// <param name="profileId">
    ///     The ID of the profile to find the picture URL for.
    /// </param>
    /// <returns>
    ///     A string containing the URL of the profile picture.
    /// </returns>
    public async Task<string> FindProfilePictureUrlByIdAsync(ObjectId profileId)
    {
        var imageUrlValueObject = await _profileCollection
            .Find(x => x.Id == profileId)
            .Project(p => p.ProfilePictureUrl)
            .FirstOrDefaultAsync();

        return imageUrlValueObject?.GetValue() ?? string.Empty;
    }

    /// <summary>
    ///      Method to find all profiles by user ID.
    /// </summary>
    /// <param name="userId">
    ///     The ID of the user to find profiles for.
    /// </param>
    /// <returns>
    ///     A list of profiles for the specified user.
    /// </returns>
    public async Task<IEnumerable<Profile>> FindAllByUserIdAsync(string userId)
    {
        return await _profileCollection
            .Find(x => x.UserId == userId)
            .ToListAsync();
    }
}