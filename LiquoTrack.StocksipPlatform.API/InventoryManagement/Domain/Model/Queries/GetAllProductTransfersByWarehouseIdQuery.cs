using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get all product transfers by warehouse ID
/// </summary>
public record GetAllProductTransfersByWarehouseIdQuery(ObjectId WarehouseId);