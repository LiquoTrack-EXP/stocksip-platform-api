using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get all product transfers for a given product ID.
/// </summary>
public record GetAllProductTransfersByProductIdQuery(ObjectId ProductId);