using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Command for registering a new product in inventory.
/// </summary>
public record RegisterProductCommand(
        string Name,
        EProductTypes Type,
        string Brand,
        Money UnitPrice,
        ProductMinimumStock MinimumStock,
        ProductContent Content,
        IFormFile? Image,
        AccountId AccountId,
        AccountId SupplierId
    );