using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.ACL;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Events;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Application.Internal.EventHandlers;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.EventHandlers;

/// <summary>
///     Handler for the <see cref="ProductWithLowStockDetectedEvent"/> event.
/// </summary>
/// <param name="alertsAndNotificationsService">
///     The external service for creating alerts and notifications.
/// </param>
public class ProductWithLowStockDetectedEventHandler(
        ExternalAlertsAndNotificationsService alertsAndNotificationsService,
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository
    ) : IEventHandler<ProductWithLowStockDetectedEvent>
{
    /// <summary>
    ///     Method to handle the event.
    /// </summary>
    /// <param name="notification">
    ///     The event notification.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    public async Task Handle(ProductWithLowStockDetectedEvent notification, CancellationToken cancellationToken)
    {
        var inventoryId = await On(notification);
        Console.WriteLine($"[ProductLowStock] Inventory ID: {inventoryId}");
    }
    
    private async Task<string> On(ProductWithLowStockDetectedEvent domainEvent)
    {
        try
        {
            Inventory? inventory;
            if (domainEvent.ExpirationDate != null)
            {
                inventory = await inventoryRepository
                    .GetByProductIdWarehouseIdAndExpirationDateAsync(
                        new ObjectId(domainEvent.ProductId),
                        new ObjectId(domainEvent.WarehouseId),
                        domainEvent.ExpirationDate
                    );
            }
            else
            {
                inventory = await inventoryRepository
                    .GetByProductIdWarehouseIdAsync(
                        new ObjectId(domainEvent.ProductId),
                        new ObjectId(domainEvent.WarehouseId)
                    );
            }

            if (inventory == null)
            {
                Console.WriteLine("[EventHandler] Inventory does not exist for event " + domainEvent.GetType().Name);
                return "EventHandler error"; // No rompemos la operación principal
            }

            var product = await productRepository.FindByIdAsync(domainEvent.ProductId.ToString());
            var warehouse = await warehouseRepository.FindByIdAsync(domainEvent.WarehouseId.ToString());

            if (product == null || warehouse == null)
            {
                Console.WriteLine("[EventHandler] Product or Warehouse missing for event " + domainEvent.GetType().Name);
                return "EventHandler error";
            }

            alertsAndNotificationsService.CreateAlert(
                title: "Low Stock Level Warning",
                message: $"Product {product.Name} in warehouse {warehouse.Name} has reached the minimum stock level.",
                severity: "Warning",
                type: "ProductLowStock",
                accountId: domainEvent.AccountId,
                inventory.Id.ToString()
            );

            return inventory.Id.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling event {domainEvent.GetType().Name}: {ex}");
            return "Error"; // No interrumpe la operación principal
        }
    }
}