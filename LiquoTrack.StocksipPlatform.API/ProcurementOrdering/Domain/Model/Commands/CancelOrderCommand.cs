namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to cancel a purchase order.
/// </summary>
/// <param name="orderId">The identifier of the purchase order to cancel.</param>
public record CancelOrderCommand(string orderId);