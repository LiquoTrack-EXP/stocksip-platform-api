using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Seeding;

public class DatabaseSeeder(
    IBrandRepository brandRepository, 
    IPlanRepository planRepository,
    ITypeRepository typeRepository)
{
    /// <summary>
    ///     Method to seed the database with initial data.
    /// </summary>
    public async Task SeedAsync()
    {
        // Seed the brand names into the database
        await brandRepository.SeedBrandNamesAsync();
        
        // Seed the plans for subscription into the database
        await planRepository.SeedPlansAsync();
        
        // Seed the product types names into the database
        await typeRepository.SeedTypesNamesAsync();
    }
}