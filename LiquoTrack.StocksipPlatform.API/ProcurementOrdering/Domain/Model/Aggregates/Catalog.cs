using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;

/// <summary>
/// Aggregate entity representing a catalog.
/// </summary>
public class Catalog : Entity
{
    [BsonIgnore]
    public CatalogId CatalogId => new(Id.ToString());
    
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<CatalogItem> CatalogItems { get; private set; } = new();
    public AccountId OwnerAccount { get; private set; }
    public Email ContactEmail { get; private set; }
    public bool IsPublished { get; private set; } = false;
    
    /// <summary>
    /// The warehouse ID from which this catalog sources its products.
    /// </summary>
    public string? WarehouseId { get; private set; }

    public Catalog(string name, string description, AccountId ownerAccount, Email contactEmail, string? warehouseId = null)
    {
        Name = ValidateName(name);
        Description = description;
        OwnerAccount = ownerAccount;
        ContactEmail = contactEmail;
        WarehouseId = warehouseId;
    }

    public Catalog(CreateCatalogCommand command)
        : this(
            command.name, 
            command.description, 
            new AccountId(command.ownerAccount), 
            new Email(command.contactEmail),
            command.warehouseId
        )
    {
    }

    [BsonConstructor]
    protected Catalog() { }

    private static string ValidateName(string name)
        => string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("The catalog name cannot be null or empty.", nameof(name))
            : name;

    public void PublishCatalog() => IsPublished = true;
    public void UnpublishCatalog() => IsPublished = false;

    public void UpdateCatalog(UpdateCatalogCommand command)
    {
        Name = ValidateName(command.name);
        Description = command.description;
        ContactEmail = new Email(command.contactEmail);
    }

    /// <summary>
    /// Sets the warehouse ID for this catalog.
    /// </summary>
    public void SetWarehouse(string warehouseId)
    {
        if (string.IsNullOrWhiteSpace(warehouseId))
            throw new ArgumentException("Warehouse ID cannot be null or empty.", nameof(warehouseId));
        
        WarehouseId = warehouseId;
    }

    /// <summary>
    /// Adds an item to the catalog using product data from another context.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="productName">The product name.</param>
    /// <param name="unitPrice">The unit price.</param>
    /// <param name="currency">The currency code.</param>
    /// <param name="imageUrl">The product image URL.</param>
    /// <param name="availableStock">The available stock quantity (optional).</param>
    public void AddItem(
        string productId, 
        string productName, 
        decimal unitPrice, 
        string currency, 
        string imageUrl,
        int? availableStock = null)
    {
        if (CatalogItems.Any(i => i.ProductId.GetId == productId))
            throw new InvalidOperationException($"Product {productId} is already in the catalog.");
        
        var money = new Money(unitPrice, new Currency(currency));
        var item = new CatalogItem(
            new ProductId(productId), 
            productName, 
            money, 
            imageUrl, 
            DateTime.UtcNow,
            availableStock
        );
        
        CatalogItems.Add(item);
    }

    /// <summary>
    /// Updates the available stock for a catalog item.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="newStock">The new stock quantity.</param>
    public void UpdateItemStock(string productId, int newStock)
    {
        var item = CatalogItems.FirstOrDefault(i => i.ProductId.GetId == productId);
        
        if (item == null)
            throw new InvalidOperationException($"Product {productId} not found in catalog.");
        
        item.UpdateStock(newStock);
    }

    public void RemoveItem(RemoveItemFromCatalogCommand command)
    {
        var item = CatalogItems.FirstOrDefault(i => i.ProductId == command.productId);
        
        if (item != null)
            CatalogItems.Remove(item);
    }
    
    /// <summary>
    /// Reduces the stock for a specific catalog item using a domain command.
    /// </summary>
    public void ReduceItemStock(ReduceCatalogItemStockCommand command)
    {
        var item = CatalogItems.FirstOrDefault(i => i.ProductId == command.ProductId);
        if (item == null)
            throw new InvalidOperationException($"Product {command.ProductId} not found in catalog.");

        if (item.Stock < command.Quantity)
            throw new InvalidOperationException(
                $"Insufficient stock for product {command.ProductId}. " +
                $"Available: {item.Stock}, requested: {command.Quantity}"
            );

        item.Stock -= command.Quantity;
    }

    public AccountId GetOwnerAccount() => OwnerAccount;
}