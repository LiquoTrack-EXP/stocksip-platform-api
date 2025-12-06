using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;

public record PurchaseOrderItem
{
    public ProductId ProductId { get; init; }
    public string ProductName { get; init; }
    public string ImageUrl { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    
    public PurchaseOrderItem(CatalogItem catalogItem, int quantity)
    {
        if (catalogItem == null)
            throw new ArgumentNullException(nameof(catalogItem));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        ProductId = catalogItem.ProductId;
        ProductName = catalogItem.ProductName;
        ImageUrl = catalogItem.ImageUrl ?? string.Empty;
        UnitPrice = catalogItem.UnitPrice.GetAmount();
        Quantity = quantity;
    }
    
    public PurchaseOrderItem(
        ProductId productId,
        string productName,
        string imageUrl,
        decimal unitPrice,
        int quantity)
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        ProductName = string.IsNullOrWhiteSpace(productName) 
            ? throw new ArgumentException("ProductName cannot be null or empty", nameof(productName)) 
            : productName;
        ImageUrl = imageUrl ?? string.Empty;
        if (unitPrice < 0)
            throw new ArgumentException("UnitPrice cannot be negative", nameof(unitPrice));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public decimal CalculateSubTotal() => UnitPrice * Quantity;
}