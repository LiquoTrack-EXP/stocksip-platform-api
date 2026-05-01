using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Events;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;

/// <summary>
///     Aggregate entity representing the inventory of a product in a warehouse.
/// </summary>
public class Inventory(
        ObjectId productId,
        ObjectId warehouseId,
        ProductStock quantity,
        ProductExpirationDate? expirationDate
    ) : Entity
{
    /// <summary>
    ///     The identifier of the product related to this inventory.
    /// </summary>
    public ObjectId ProductId { get; private set; } = productId;

    /// <summary>
    ///     The identifier of the warehouse that owns the product.
    /// </summary>
    public ObjectId WarehouseId { get; private set; } = warehouseId;
    
    /// <summary>
    ///     The quantity of the product in the warehouse.
    /// </summary>
    public ProductStock Quantity { get; private set; } = quantity;
    
    /// <summary>
    ///     The current state of the product.
    /// </summary>
    public EProductStates CurrentState { get; private set; } = EProductStates.WithStock;
    
    /// <summary>
    ///     The expiration date of the product.
    /// </summary>
    [BsonIgnoreIfNull]
    public ProductExpirationDate? ExpirationDate { get; private set; } = expirationDate;
    
    /// <summary>
    ///     Marks the product as out of stock.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     Thrown when the product is already out of stock.
    /// </exception>
    private void SetProductStateToOutOfStock()
    {
        // Check if the product is already out of stock
        if (CurrentState == EProductStates.OutOfStock)
        {
            throw new ArgumentException("Product is already out of stock");
        }
        
        // Set the product state to out of stock
        CurrentState = EProductStates.OutOfStock;
    }
    
    /// <summary>
    ///     Marks the product as low of stock.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     Thrown when the product is already low of stock.
    /// </exception>
    private void SetProductStateToLowStock()
    {
        // Check if the product is already out of stock
        if (CurrentState == EProductStates.LowStock)
        {
            throw new ArgumentException("Product is already low of stock");
        }
        
        // Set the product state to out of stock
        CurrentState = EProductStates.LowStock;
    }

    /// <summary>
    ///     Marks the product as with stock.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     Thrown when the product is already marked as with stock.
    /// </exception>
    private void SetProductStateToWithStock()
    {
        // Check if the product is already marked as with stock
        if (CurrentState == EProductStates.WithStock)
        {
            throw new ArgumentException("Product is already marked as with stock");
        }
        
        // Set the product state to with stock
        CurrentState = EProductStates.WithStock;
    }
    
    

    /// <summary>
    ///     Adds stock to the product in the inventory.
    /// </summary>
    /// <param name="addedStock">
    ///     The amount of stock to be added to the product.
    /// </param>
    /// <param name="productMinimumStock">
    ///     The minimum stock of the product related to this inventory.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the added stock is less than or equal to zero.
    /// </exception>
    public void AddStockToProduct(int addedStock, int productMinimumStock)
    {
        if (addedStock <= 0) throw new ArgumentException("Stock cannot be negative");
        
        var newStock = Quantity.GetValue + addedStock;
        
        if (newStock > productMinimumStock && CurrentState != EProductStates.WithStock)
        {
            SetProductStateToWithStock();
        }
        else if (newStock > 0 && newStock <= productMinimumStock && CurrentState != EProductStates.LowStock)
        {
            SetProductStateToLowStock();
        }
        
        Quantity = Quantity.AddStock(addedStock);
    }

    /// <summary>
    ///     Removes stock from the product in the inventory.
    /// </summary>
    /// <param name="removedStock">
    ///     The amount of stock to be removed from the product.
    /// </param>
    /// <param name="productMinimumStock">
    ///     The minimum stock of the product related to this inventory.
    /// </param>
    /// <param name="accountId">
    ///     The account ID of the warehouse that owns the product.
    ///     It is used to generate an alert when the product is below the minimum stock after removal.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the removed stock is less than or equal to zero, or when there are not enough stocks to remove.
    /// </exception>
    public void DecreaseStockFromProduct(int removedStock, int productMinimumStock, AccountId accountId)
    {
        // Validate the removed stock amount
        if (removedStock <= 0) throw new ArgumentException("Stock cannot be negative");
        
        // Check if there are enough stocks to remove
        if (Quantity.GetValue < removedStock) throw new ArgumentException("Insufficient stock to remove");

        // Checks if the product is below minimum stock after removal. If so, it should trigger a domain event to generate an alert.
        if (productMinimumStock >= Quantity.GetValue - removedStock && CurrentState != EProductStates.LowStock && CurrentState != EProductStates.OutOfStock)
        {
            // Sets the state of the product to low stock
            SetProductStateToLowStock();
            
            // Create a domain event to notify about the product problem by creating an alert in the alerts and notifications context.
            var productProblemEvent = new ProductWithLowStockDetectedEvent(
                    accountId.GetId,
                    ProductId.ToString(),
                    WarehouseId.ToString(),
                    ExpirationDate ?? new ProductExpirationDate()
                );

            AddDomainEvent(productProblemEvent);
        }
        
        // Checks if the product stock is zero after the removal. If so, it should trigger a domain event to generate an alert.
        if (Quantity.GetValue - removedStock == 0 && CurrentState != EProductStates.OutOfStock)
        {
            // Sets the state of the product to out of stock
            SetProductStateToOutOfStock();
            
            // Create a domain event to notify about the product problem by creating an alert in the alerts and notifications context.
            var productProblemEvent = new ProductWithoutStockDetectedEvent(
                accountId.GetId,
                ProductId.ToString(),
                WarehouseId.ToString(),
                ExpirationDate
            );

            AddDomainEvent(productProblemEvent);
        }
        
        // Decrease the stock of the product
        Quantity = Quantity.DecreaseStock(removedStock);
    }
    
    /// <summary>
    ///     Getter method for the total stock in the inventory.
    /// </summary>
    /// <returns>
    ///     The total stock in the inventory.
    /// </returns>
    public int GetStock() => Quantity.GetValue;
}