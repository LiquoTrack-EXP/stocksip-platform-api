
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;

/// <summary>
/// Interface for catalog command service.
/// </summary>
public interface ICatalogCommandService
{
    /// <summary>
    /// Handles the CreateCatalogCommand.
    /// </summary>
    /// <param name="command">The command to create a catalog.</param>
    /// <returns>The identifier of the created catalog.</returns>
    Task<CatalogId> Handle(CreateCatalogCommand command);

    /// <summary>
    /// Handles the UpdateCatalogCommand.
    /// </summary>
    /// <param name="command">The command to update a catalog.</param>
    Task Handle(UpdateCatalogCommand command);

    /// <summary>
    /// Handles the PublishCatalogCommand.
    /// </summary>
    /// <param name="command">The command to publish a catalog.</param>
    Task Handle(PublishCatalogCommand command);

    /// <summary>
    /// Handles the UnpublishCatalogCommand.
    /// </summary>
    /// <param name="command">The command to unpublish a catalog.</param>
    Task Handle(UnpublishCatalogCommand command);

    /// <summary>
    /// Handles the AddItemToCatalogCommand.
    /// </summary>
    /// <param name="command">The command to add an item to a catalog.</param>
    Task Handle(AddItemToCatalogCommand command);

    /// <summary>
    /// Handles the RemoveItemFromCatalogCommand.
    /// </summary>
    /// <param name="command">The command to remove an item from a catalog.</param>
    Task Handle(RemoveItemFromCatalogCommand command);
    
    /// <summary>
    /// Handles the ReduceCatalogItemStockCommand.
    /// </summary>
    /// <param name="command">The command to reduce stock for a catalog item.</param>
    Task Handle(ReduceCatalogItemStockCommand command);
}