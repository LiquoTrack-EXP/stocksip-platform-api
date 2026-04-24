using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Command to update product information.
/// </summary>
public record UpdateProductInformationCommand(
        ObjectId ProductId,
        string? Name,
        Money? UnitPrice,
        ProductMinimumStock? MinimumStock,
        ProductContent? Content,
        IFormFile? Image
    );