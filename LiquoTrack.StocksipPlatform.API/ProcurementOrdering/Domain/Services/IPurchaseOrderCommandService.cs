
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;

/// <summary>
/// Interface for purchase order command service.
/// </summary>
public interface IPurchaseOrderCommandService
{
    /// <summary>
    /// Handles the CreatePurchaseOrderCommand.
    /// </summary>
    /// <param name="command">The command to create a purchase order.</param>
    /// <returns>The identifier of the created purchase order.</returns>
    Task<PurchaseOrderId> Handle(CreatePurchaseOrderCommand command);

    /// <summary>
    /// Handles the AddItemToOrderCommand.
    /// </summary>
    /// <param name="command">The command to add an item to an order.</param>
    Task Handle(AddItemToOrderCommand command);

    /// <summary>
    /// Handles the RemoveItemFromOrderCommand.
    /// </summary>
    /// <param name="command">The command to remove an item from an order.</param>
    Task Handle(RemoveItemFromOrderCommand command);

    /// <summary>
    /// Handles the ConfirmOrderCommand.
    /// </summary>
    /// <param name="command">The command to confirm an order.</param>
    Task Handle(ConfirmOrderCommand command);

    /// <summary>
    /// Handles the ShipOrderCommand.
    /// </summary>
    /// <param name="command">The command to ship an order.</param>
    Task Handle(ShipOrderCommand command);

    /// <summary>
    /// Handles the ReceiveOrderCommand.
    /// </summary>
    /// <param name="command">The command to receive an order.</param>
    Task Handle(ReceiveOrderCommand command);

    /// <summary>
    /// Handles the CancelOrderCommand.
    /// </summary>
    /// <param name="command">The command to cancel an order.</param>
    Task Handle(CancelOrderCommand command);
}