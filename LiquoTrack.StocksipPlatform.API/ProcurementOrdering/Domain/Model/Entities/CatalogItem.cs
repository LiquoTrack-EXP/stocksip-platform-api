using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;

/// <summary>
/// Entity representing an item within a catalog.
/// </summary>
public class CatalogItem : Entity
{
    /// <summary>
    /// The product identifier.
    /// </summary>
    public ProductId ProductId { get; private set; }
    
    /// <summary>
    /// The product name.
    /// </summary>
    public string ProductName { get; private set; }
    
    /// <summary>
    /// The unit price of the product.
    /// </summary>
    public Money UnitPrice { get; private set; }
    
    /// <summary>
    /// The product image URL.
    /// </summary>
    public string ImageUrl { get; private set; }
    
    /// <summary>
    /// The date when the product was added to the catalog.
    /// </summary>
    public DateTime AddedAt { get; private set; }
    
    /// <summary>
    /// The available stock for this product in the associated warehouse.
    /// Can be null if stock tracking is not enabled for this catalog.
    /// </summary>
    public int? AvailableStock { get; private set; }

    /// <summary>
    /// Provides access to the current stock value. 
    /// Returns 0 if stock tracking is disabled.
    /// </summary>
    [BsonIgnore]
    public int Stock
    {
        get => AvailableStock ?? 0;
        internal set
        {
            if (value < 0)
                throw new ArgumentException("Stock cannot be negative.", nameof(value));

            AvailableStock = value;
        }
    }

    /// <summary>
    /// Creates a new catalog item.
    /// </summary>
    public CatalogItem(
        ProductId productId, 
        string productName, 
        Money unitPrice, 
        string imageUrl, 
        DateTime addedAt,
        int? availableStock = null)
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        ProductName = !string.IsNullOrWhiteSpace(productName)
            ? productName
            : throw new ArgumentException("Product name cannot be null or empty.", nameof(productName));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        ImageUrl = imageUrl;
        AddedAt = addedAt;
        
        if (availableStock.HasValue && availableStock.Value < 0)
            throw new ArgumentException("Available stock cannot be negative.", nameof(availableStock));
        
        AvailableStock = availableStock;
    }

    /// <summary>
    /// Updates the available stock for this catalog item.
    /// </summary>
    public void UpdateStock(int newStock)
    {
        if (newStock < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(newStock));
        
        AvailableStock = newStock;
    }

    /// <summary>
    /// Checks if the item has sufficient stock for a given quantity.
    /// </summary>
    public bool HasSufficientStock(int requestedQuantity)
    {
        if (AvailableStock == null)
            return true;
        
        return AvailableStock >= requestedQuantity;
    }
    
    /// <summary>
    /// Reduces the stock of this item by the specified quantity.
    /// Throws an exception if there is insufficient stock.
    /// </summary>
    public void ReduceStock(int quantity)
    {
        if (AvailableStock == null)
            return; // Stock tracking disabled

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (!HasSufficientStock(quantity))
            throw new InvalidOperationException(
                $"Insufficient stock for product '{ProductName}'. Available: {AvailableStock}, Requested: {quantity}");

        AvailableStock -= quantity;
    }

    /// <summary>
    /// Calculates the subtotal for a given quantity of this item.
    /// </summary>
    public decimal CalculateSubTotal(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        
        return UnitPrice.GetAmount() * quantity;
    }

    // Protected constructor for MongoDB
    protected CatalogItem() { }
}
