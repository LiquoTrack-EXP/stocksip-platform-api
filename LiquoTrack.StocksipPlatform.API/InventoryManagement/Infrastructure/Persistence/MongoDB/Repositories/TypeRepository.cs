using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Repository implementation for the Product Type entity.
/// </summary>
public class TypeRepository(AppDbContext context, IMediator mediator) : BaseRepository<ProductType>(context, mediator), ITypeRepository
{
    /// <summary>
    ///     The MongoDB collection for the Product Type entity.   
    /// </summary>
    private readonly IMongoCollection<ProductType> _typeCollection = context.GetCollection<ProductType>();
    
    /// <summary>
    ///     Method to seed the product types in the database.
    /// </summary>
    /// <returns>
    ///     A confirmation of the seeding operation.
    /// </returns>
    public async Task SeedTypesNamesAsync()
    {
        // Get the list of brand names from the enum
        var nameList = Enum.GetValues<EProductTypes>()
            .Select(m => m.ToString())
            .ToList();

        // Verifies which names already exist in the database
        var existingNames = await _typeCollection
            .Find(FilterDefinition<ProductType>.Empty)
            .Project(m => m.Name)
            .ToListAsync();
        
        // Determine which names need to be added
        var namesToAdd = nameList
            .Where(name => !existingNames.Contains(Enum.Parse<EProductTypes>(name)))
            .Select(name => new ProductType(Enum.Parse<EProductTypes>(name)))
            .ToList();

        // Insert the new names into the database
        if (namesToAdd.Count != 0) await _typeCollection.InsertManyAsync(namesToAdd);
    }
}