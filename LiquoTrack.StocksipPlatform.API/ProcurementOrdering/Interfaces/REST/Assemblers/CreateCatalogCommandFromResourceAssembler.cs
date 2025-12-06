using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Assemblers;

/// <summary>
/// Assembler to convert CreateCatalogResource to CreateCatalogCommand.
/// </summary>
public static class CreateCatalogCommandFromResourceAssembler
{
    /// <summary>
    /// Converts a CreateCatalogResource to a CreateCatalogCommand.
    /// </summary>
    /// <param name="resource">The creation catalog resource.</param>
    /// <returns>The create catalog command.</returns>
    public static CreateCatalogCommand ToCommandFromResource(CreateCatalogResource resource)
    {
        return new CreateCatalogCommand(
            resource.name,
            resource.description,
            resource.ownerAccount,
            resource.contactEmail
        );
    }
}