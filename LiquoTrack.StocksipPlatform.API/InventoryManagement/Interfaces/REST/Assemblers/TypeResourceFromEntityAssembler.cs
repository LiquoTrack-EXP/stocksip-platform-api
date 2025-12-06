using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using Microsoft.OpenApi.Extensions;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Static assembler class to convert ProductType entity to TypeResource.
/// </summary>
public static class TypeResourceFromEntityAssembler
{
    /// <summary>
    ///     Static method to convert ProductType entity to TypeResource.
    /// </summary>
    /// <param name="entity">
    ///     The ProductType entity to convert.
    /// </param>
    /// <returns>
    ///     The TypeResource representation of the ProductType entity.
    /// </returns>
    public static TypeResource ToResourceFromEntity(ProductType entity)
    {
        return new TypeResource(entity.Name.GetDisplayName());
    }
}