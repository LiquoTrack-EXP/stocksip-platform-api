using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get a product transfer by product ID and warehouse ID.
/// </summary>
public record GetProductTransferByProductIdAndWarehouseIdQuery(ObjectId ProductId, ObjectId WarehouseId, DateTime? ExpirationDate);