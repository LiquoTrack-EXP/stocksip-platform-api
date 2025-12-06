using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get an inventory by its ID.
/// </summary>
public record GetInventoryByIdQuery(ObjectId InventoryId);