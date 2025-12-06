using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get a product exit by product ID and warehouse ID.
/// </summary>
public record GetProductExitByProductIdAndWarehouseIdQuery(ObjectId ProductId, ObjectId WarehouseId, DateTime? ExpirationDate);