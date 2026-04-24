using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;

/// <summary>
///     Query to get a product transfer by its ID.
/// </summary>
public record GetProductTransferByIdQuery(ObjectId ProductTransferId);