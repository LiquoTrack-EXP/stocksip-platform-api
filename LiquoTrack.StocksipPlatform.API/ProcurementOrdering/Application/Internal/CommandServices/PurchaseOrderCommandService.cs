using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.Internal.CommandServices;

/// <summary>
/// Application service responsible for handling all commands related to <see cref="PurchaseOrder"/>.
/// Implements business logic at the application layer and delegates persistence to repositories.
/// </summary>
public class PurchaseOrderCommandService : IPurchaseOrderCommandService
{
    private readonly IPurchaseOrderRepository _orderRepository;
    private readonly ICatalogRepository _catalogRepository;
    private readonly ICatalogCommandService _catalogCommandService;
    private readonly IPaymentAndSubscriptionsFacade _paymentFacade;

    public PurchaseOrderCommandService(
        IPurchaseOrderRepository orderRepository,
        ICatalogRepository catalogRepository,
        ICatalogCommandService catalogCommandService,
        IPaymentAndSubscriptionsFacade paymentFacade)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _catalogRepository = catalogRepository ?? throw new ArgumentNullException(nameof(catalogRepository));
        _catalogCommandService = catalogCommandService ?? throw new ArgumentNullException(nameof(catalogCommandService));
        _paymentFacade = paymentFacade ?? throw new ArgumentNullException(nameof(paymentFacade));
    }

    /// <summary>
    /// Handles the CreatePurchaseOrderCommand.
    /// Creates a new purchase order and optionally assigns a delivery address if addressIndex is provided.
    /// </summary>
    public async Task<PurchaseOrderId> Handle(CreatePurchaseOrderCommand command)
    {
        var order = new PurchaseOrder(command);

        if (command.addressIndex.HasValue)
        {
            var address = await _paymentFacade.GetAccountAddressAsync(
                command.buyer,
                command.addressIndex.Value
            );

            if (address == null)
                throw new InvalidOperationException(
                    $"Address at index {command.addressIndex.Value} not found for account {command.buyer}.");

            var deliveryAddress = DeliveryAddress.FromAddress(address);
            order.SetDeliveryAddress(deliveryAddress);
        }

        await _orderRepository.AddAsync(order);
        return order.PurchaseOrderId;
    }

    /// <summary>
    /// Handles the AddItemToOrderCommand.
    /// Adds a catalog item to an existing purchase order with the specified quantity.
    /// Also reduces the available stock of that item in the catalog.
    /// </summary>
    public async Task Handle(AddItemToOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.OrderId);

        var catalog = await _catalogRepository.GetByIdAsync(order.CatalogIdBuyFrom)
                      ?? throw new InvalidOperationException("Associated catalog not found.");

        var catalogItem = catalog.CatalogItems
            .FirstOrDefault(i => i.ProductId.GetId.Trim().ToLower() == command.ProductId.Trim().ToLower())
            ?? throw new InvalidOperationException($"Product with ID '{command.ProductId}' not found in catalog.");
        
        if (!catalogItem.HasSufficientStock(command.Quantity))
            throw new InvalidOperationException($"Insufficient stock for product '{catalogItem.ProductName}'.");
        
        order.AddItem(catalogItem, command.Quantity);
        await _orderRepository.UpdateAsync(order);
        
        var reduceStockCommand = new ReduceCatalogItemStockCommand(
            catalog.CatalogId.GetId(),
            catalogItem.ProductId,
            command.Quantity
        );

        await _catalogCommandService.Handle(reduceStockCommand);
    }

    /// <summary>
    /// Handles the RemoveItemFromOrderCommand.
    /// </summary>
    public async Task Handle(RemoveItemFromOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.orderId);
        order.RemoveItem(command);
        await _orderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// Handles the ConfirmOrderCommand.
    /// </summary>
    public async Task Handle(ConfirmOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.orderId);
        order.ConfirmOrder();
        await _orderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// Handles the ShipOrderCommand.
    /// </summary>
    public async Task Handle(ShipOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.orderId);
        order.ShipOrder();
        await _orderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// Handles the ReceiveOrderCommand.
    /// </summary>
    public async Task Handle(ReceiveOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.orderId);
        order.ReceiveOrder();
        await _orderRepository.UpdateAsync(order);
    }

    /// <summary>
    /// Handles the CancelOrderCommand.
    /// </summary>
    public async Task Handle(CancelOrderCommand command)
    {
        var order = await GetOrderByIdAsync(command.orderId);
        order.CancelOrder();
        await _orderRepository.UpdateAsync(order);
    }

    private async Task<PurchaseOrder> GetOrderByIdAsync(string orderId)
    {
        var id = new PurchaseOrderId(orderId);
        var order = await _orderRepository.GetByIdAsync(id);
        return order ?? throw new InvalidOperationException($"Order with ID '{orderId}' not found.");
    }
}