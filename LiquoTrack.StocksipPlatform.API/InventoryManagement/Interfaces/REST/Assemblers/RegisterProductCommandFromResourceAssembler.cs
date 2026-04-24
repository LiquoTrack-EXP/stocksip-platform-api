using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Static assembler class to convert RegisterProductResource to RegisterProductCommand.
/// </summary>
public static class RegisterProductCommandFromResourceAssembler
{
    /// <summary>
    ///     Static method to convert RegisterProductResource to RegisterProductCommand.   
    /// </summary>
    /// <param name="resource">
    ///     The RegisterProductResource to convert.
    /// </param>
    /// <param name="accountId">
    ///     The account id of the user registering the product.
    /// </param>
    /// <returns>
    ///     A RegisterProductCommand representation of the RegisterProductResource.
    /// </returns>
    public static RegisterProductCommand ToCommandFromResource(RegisterProductResource resource, string accountId)
    {
        return new RegisterProductCommand(
                resource.Name,
                Enum.Parse<EProductTypes>(resource.Type),
                resource.Brand,
                new Money(resource.UnitPrice, new Currency(resource.Code)),
                new ProductMinimumStock(resource.MinimumStock),
                new ProductContent(resource.Content),
                resource.Image,
                new AccountId(accountId),
                new AccountId(resource.SupplierId ?? "")
            );
    }
}