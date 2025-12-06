using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;

/// <summary>
///     Interface for managing ProductType entities.
/// </summary>
public interface ITypeRepository : IBaseRepository<ProductType>
{
    /// <summary>
    ///     Method to seed the product types in the database.
    /// </summary>
    /// <returns>
    ///     A confirmation of the seeding operation.
    /// </returns>
    Task SeedTypesNamesAsync();
}