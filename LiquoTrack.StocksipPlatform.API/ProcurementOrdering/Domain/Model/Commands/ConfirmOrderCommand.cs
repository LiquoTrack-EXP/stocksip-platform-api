namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;

/// <summary>
/// Command to confirm a purchase order.
/// </summary>
/// <param name="orderId">The identifier of the purchase order to confirm.</param>
public record ConfirmOrderCommand(string orderId);