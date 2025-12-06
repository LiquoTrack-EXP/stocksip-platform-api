using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;

/// <summary>
///     Command to decrease products from a warehouse without expiration date.
/// </summary>
public record DecreaseProductsFromWarehouseWithoutExpirationDateCommand(ObjectId ProductId, ObjectId WarehouseId, int QuantityToDecrease, EProductExitReasons ExitType);