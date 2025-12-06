namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Application.Internal.CommandServices;

using System.ComponentModel.DataAnnotations;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using CatalogId = LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.CatalogId;
using PurchaseOrderId = LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.PurchaseOrderId;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.ACL;

public class SalesOrderCommandService : ISalesOrderCommandService
{
    private readonly ISalesOrderRepository salesOrderRepository;
    private readonly ILowStockService lowStockService;
    private readonly ICatalogQueryService catalogQueryService;
    private readonly IProcurementOrderingFacade procurementOrderingFacade;

    public SalesOrderCommandService(
        ISalesOrderRepository salesOrderRepository,
        ILowStockService lowStockService,
        ICatalogQueryService catalogQueryService,
        IProcurementOrderingFacade procurementOrderingFacade)
    {
        this.salesOrderRepository = salesOrderRepository ?? throw new ArgumentNullException(nameof(salesOrderRepository));
        this.lowStockService = lowStockService ?? throw new ArgumentNullException(nameof(lowStockService));
        this.catalogQueryService = catalogQueryService ?? throw new ArgumentNullException(nameof(catalogQueryService));
        this.procurementOrderingFacade = procurementOrderingFacade ?? throw new ArgumentNullException(nameof(procurementOrderingFacade));
    }

    public async Task<SalesOrder> Handle(GenerateSalesOrderCommand command)
    {
        if (command.items == null || !command.items.Any())
            throw new ValidationException("Cannot create an order with no items");
        
        foreach (var item in command.items)
        {
            if (item.InventoryId == null)
                item.InventoryId = new InventoryId(Guid.NewGuid().ToString());
        }

        var order = await salesOrderRepository.GenerateSalesOrder(command);

        try
        {
            var catalog = await catalogQueryService.Handle(
                new LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries.GetCatalogByIdQuery(command.catalogToBuyFrom.GetId())
            );

            var supplierOwner = catalog?.GetOwnerAccount();
            if (supplierOwner != null)
            {
                order.SetSupplier(supplierOwner);
            }
        }
        catch { }

        order.UpdateStatus(ESalesOrderStatuses.Processing, "Order pending");
        await salesOrderRepository.AddAsync(order);

        return order;
    }

    public async Task<SalesOrder> GeneratePurchaseOrder(GeneratePurchaseOrderRequest request, string accountId)
    {
        if (!request.IsAutomatic && (!request.Items.Any() || request.Items.All(i => i.QuantityToSell <= 0)))
            throw new ValidationException("Cannot create an order with no items");

        ICollection<SalesOrderItem> items = request.IsAutomatic
            ? await GetLowStockItems(accountId, request.CatalogToBuyFrom)
            : request.Items.Select(i =>
            {
                var zeroMoney = new Money(0, new Currency(EValidCurrencyCodes.USD.ToString()));
                return new SalesOrderItem(new ProductId(i.ProductId), zeroMoney, i.QuantityToSell, i.ProductName)
                {
                    InventoryId = string.IsNullOrEmpty(i.InventoryId)
                        ? new InventoryId(Guid.NewGuid().ToString())
                        : new InventoryId(i.InventoryId!)
                };
            }).ToList();

        if (!items.Any())
            throw new InvalidOperationException("No items available for order generation");

        var command = new GenerateSalesOrderCommand(
            orderCode: request.OrderCode,
            purchaseOrderId: new PurchaseOrderId(Guid.NewGuid().ToString()),
            items: items,
            status: ESalesOrderStatuses.Processing,
            catalogToBuyFrom: new CatalogId(request.CatalogToBuyFrom),
            receiptDate: request.ReceiptDate ?? DateTime.UtcNow.AddDays(7),
            completitionDate: request.CompletitionDate ?? DateTime.UtcNow.AddDays(14),
            accountId: new AccountId(accountId)
        );

        var created = await Handle(command);

        if (created.SupplierId == null)
        {
            try
            {
                var catalog = await catalogQueryService.Handle(
                    new LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Queries.GetCatalogByIdQuery(request.CatalogToBuyFrom)
                );
                var supplierOwner = catalog?.GetOwnerAccount();
                if (supplierOwner != null)
                {
                    created.SetSupplier(supplierOwner);
                    await salesOrderRepository.UpdateAsync(created);
                }
            }
            catch { }
        }

        return created;
    }

    private async Task<ICollection<SalesOrderItem>> GetLowStockItems(string accountId, string catalogId)
    {
        var lowStockItems = await lowStockService.GetLowStockItems(accountId, catalogId);
        return lowStockItems
            .Where(i => i != null && !string.IsNullOrWhiteSpace(i.ProductId) && i.SuggestedQuantity > 0)
            .Select(i => new SalesOrderItem(
                new ProductId(i.ProductId),
                new Money(0, new Currency(EValidCurrencyCodes.USD.ToString())),
                i.SuggestedQuantity,
                i.ProductName)
            {
                InventoryId = new InventoryId(Guid.NewGuid().ToString())
            }).ToList();
    }

    public async Task<SalesOrder> CreateFromPurchaseOrderAsync(string purchaseOrderId)
    {
        if (string.IsNullOrWhiteSpace(purchaseOrderId))
            throw new ValidationException("PurchaseOrderId is required");

        var purchaseOrder = await procurementOrderingFacade.GetPurchaseOrderResourceAsync(purchaseOrderId)
                            ?? throw new InvalidOperationException($"Purchase order {purchaseOrderId} not found");

        var items = purchaseOrder.items.Select(i => 
        {
            var soItem = new SalesOrderItem(
                new ProductId(i.productId),
                new Money(i.unitPrice, new Currency(EValidCurrencyCodes.USD.ToString())),
                i.quantity,
                i.productName
            );
            
            soItem.InventoryId = new InventoryId(Guid.NewGuid().ToString());
            return soItem;
        }).ToList();

        var command = new GenerateSalesOrderCommand(
            orderCode: purchaseOrder.orderCode,
            purchaseOrderId: new PurchaseOrderId(purchaseOrder.id),
            items: items,
            status: ESalesOrderStatuses.Processing,
            catalogToBuyFrom: new CatalogId(purchaseOrder.catalogIdBuyFrom),
            receiptDate: purchaseOrder.generationDate,
            completitionDate: purchaseOrder.confirmationDate ?? DateTime.UtcNow.AddDays(7),
            accountId: new AccountId(purchaseOrder.buyer)
        );

        return await Handle(command);
    }

    public async Task<SalesOrder> Handle(UpdateOrderStatusCommand command)
    {
        var order = await salesOrderRepository.GetByIdAsync(command.OrderId)
                    ?? throw new KeyNotFoundException($"Order with ID {command.OrderId} not found");

        if (order.Status == ESalesOrderStatuses.Canceled)
            throw new ValidationException("Canceled orders cannot be modified");

        if (order.DeliveryProposal == null || order.DeliveryProposal.Status != DeliveryProposalStatus.Accepted)
            throw new ValidationException("State changes require prior consent from the Liquor Store Owner (accepted delivery proposal).");

        order.UpdateStatus(command.NewStatus, command.Reason);
        await salesOrderRepository.UpdateAsync(order);

        return order;
    }

    public async Task<SalesOrder> Handle(ProposeDeliveryScheduleCommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var order = await salesOrderRepository.GetByIdAsync(command.OrderId)
                    ?? throw new KeyNotFoundException($"Order with ID {command.OrderId} not found");

        order.ProposeDelivery(command.ProposedDate, command.Notes);
        await salesOrderRepository.UpdateAsync(order);
        return order;
    }

    public async Task<SalesOrder> Handle(RespondDeliveryProposalCommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var order = await salesOrderRepository.GetByIdAsync(command.OrderId)
                    ?? throw new KeyNotFoundException($"Order with ID {command.OrderId} not found");

        if (command.Accept)
            order.AcceptDeliveryProposal(command.Notes);
        else
            order.RejectDeliveryProposal(command.Notes);

        await salesOrderRepository.UpdateAsync(order);
        return order;
    }
    
    public async Task<SalesOrder> ConfirmOrderAsync(string orderId)
    {
        var order = await GetOrderForStatusChange(orderId);
        order.UpdateStatus(ESalesOrderStatuses.Confirmed, "Order confirmed");
        await salesOrderRepository.UpdateAsync(order);
        return order;
    }

    public async Task<SalesOrder> ReceiveOrderAsync(string orderId)
    {
        var order = await GetOrderForStatusChange(orderId);
        order.UpdateStatus(ESalesOrderStatuses.Received, "Order received");
        await salesOrderRepository.UpdateAsync(order);
        return order;
    }

    public async Task<SalesOrder> ShipOrderAsync(string orderId)
    {
        var order = await GetOrderForStatusChange(orderId);
        order.UpdateStatus(ESalesOrderStatuses.Deliverying, "Order shipped");
        await salesOrderRepository.UpdateAsync(order);
        return order;
    }

    public async Task<SalesOrder> CancelOrderAsync(string orderId)
    {
        var order = await salesOrderRepository.GetByIdAsync(orderId)
                    ?? throw new KeyNotFoundException($"Order with ID {orderId} not found");

        if (order.Status == ESalesOrderStatuses.Canceled)
            throw new ValidationException("Order is already canceled");

        order.UpdateStatus(ESalesOrderStatuses.Canceled, "Order canceled");
        await salesOrderRepository.UpdateAsync(order);
        return order;
    }
    
    private async Task<SalesOrder> GetOrderForStatusChange(string orderId)
    {
        var order = await salesOrderRepository.GetByIdAsync(orderId)
                    ?? throw new KeyNotFoundException($"Order with ID {orderId} not found");

        if (order.Status == ESalesOrderStatuses.Canceled)
            throw new ValidationException("Cannot modify a canceled order");

        if (order.DeliveryProposal != null && order.DeliveryProposal.Status != DeliveryProposalStatus.Accepted)
            throw new ValidationException("State changes require an accepted delivery proposal.");

        return order;
    }
}
