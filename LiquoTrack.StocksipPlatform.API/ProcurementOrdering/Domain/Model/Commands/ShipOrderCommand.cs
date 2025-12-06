namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to mark a purchase order as shipped.
/// </summary>
/// <param name="orderId">The identifier of the purchase order to ship.</param>
public record ShipOrderCommand(string orderId);