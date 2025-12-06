using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get all product exits by product ID
/// </summary>
public record GetAllProductExitsByProductIdQuery(ObjectId ProductId);