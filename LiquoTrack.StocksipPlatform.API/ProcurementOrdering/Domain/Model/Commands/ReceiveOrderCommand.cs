namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to mark a purchase order as received.
/// </summary>
/// <param name="orderId">The identifier of the purchase order to receive.</param>
public record ReceiveOrderCommand(string orderId);