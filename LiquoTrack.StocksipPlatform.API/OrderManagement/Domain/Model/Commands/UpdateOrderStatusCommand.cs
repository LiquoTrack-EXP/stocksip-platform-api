using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;

/// <summary>
/// Command to update the status of an order
/// </summary>
/// <param name="OrderId">The ID of the order to update</param>
/// <param name="NewStatus">The new status to set</param>
public record UpdateOrderStatusCommand(
    string OrderId,
    ESalesOrderStatuses NewStatus,
    string? Reason = null
);
