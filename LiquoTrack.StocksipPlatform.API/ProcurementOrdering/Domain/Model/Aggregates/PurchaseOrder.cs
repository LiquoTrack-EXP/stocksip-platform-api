using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;

public class PurchaseOrder : Entity, IConfirmable
{
    [BsonIgnore]
    public PurchaseOrderId PurchaseOrderId => new(Id.ToString());

    public string OrderCode { get; private set; }
    public List<PurchaseOrderItem> Items { get; private set; } = new();
    public EOrderStatus Status { get; private set; } = EOrderStatus.Processing;
    public CatalogId CatalogIdBuyFrom { get; private set; }
    public DateTime GenerationDate { get; private set; } = DateTime.UtcNow;
    public DateTime? ConfirmationDate { get; private set; }
    public AccountId Buyer { get; private set; }
    public bool IsOrderSent { get; private set; } = false;
    
    /// <summary>
    ///     The delivery address for this purchase order.
    /// </summary>
    public DeliveryAddress? DeliveryAddress { get; private set; }

    public PurchaseOrder(string orderCode, CatalogId catalogIdBuyFrom, AccountId buyer)
    {
        OrderCode = ValidateOrderCode(orderCode);
        CatalogIdBuyFrom = catalogIdBuyFrom;
        Buyer = buyer;
    }

    public PurchaseOrder(CreatePurchaseOrderCommand command)
        : this(command.orderCode, new CatalogId(command.catalogIdBuyFrom), new AccountId(command.buyer))
    { }

    [BsonConstructor]
    protected PurchaseOrder() { }

    private static string ValidateOrderCode(string orderCode)
        => string.IsNullOrWhiteSpace(orderCode)
            ? throw new ArgumentException("The order code cannot be null or empty.", nameof(orderCode))
            : orderCode;

    /// <summary>
    ///     Sets the delivery address for the purchase order.
    /// </summary>
    public void SetDeliveryAddress(DeliveryAddress address)
    {
        DeliveryAddress = address ?? throw new ArgumentNullException(nameof(address));
    }

    /// <summary>
    /// Adds a single catalog item to the order with quantity.
    /// </summary>
    public void AddItem(CatalogItem catalogItem, int quantity)
    {
        if (Status != EOrderStatus.Processing)
            throw new InvalidOperationException("Cannot add items to a non-processing order.");

        if (catalogItem == null)
            throw new ArgumentNullException(nameof(catalogItem));

        var item = new PurchaseOrderItem(catalogItem, quantity);
        Items.Add(item);
    }

    /// <summary>
    /// Adds multiple items from a catalog with default quantity.
    /// </summary>
    public void AddItemsFromCatalog(IEnumerable<CatalogItem> catalogItems, int defaultQuantity = 1)
    {
        if (Status != EOrderStatus.Processing)
            throw new InvalidOperationException("Cannot add items from catalog to a non-processing order.");

        if (catalogItems == null || !catalogItems.Any())
            throw new ArgumentException("The catalog has no items to add.", nameof(catalogItems));

        foreach (var catalogItem in catalogItems)
        {
            AddItem(catalogItem, defaultQuantity);
        }
    }

    public void RemoveItem(RemoveItemFromOrderCommand command)
    {
        if (Status != EOrderStatus.Processing)
            throw new InvalidOperationException("Cannot remove items from a non-processing order.");

        var productId = new ProductId(command.productId);
        var item = Items.FirstOrDefault(i => i.ProductId.GetId.Equals(productId.GetId, StringComparison.OrdinalIgnoreCase));
        if (item != null)
            Items.Remove(item);
    }

    public Money CalculateTotal()
    {
        var total = Items.Sum(item => item.CalculateSubTotal());
        return new Money(total, new Currency("USD"));
    }

    public void ProcessOrder() => Status = EOrderStatus.Processing;

    public void ConfirmOrder()
    {
        if (Status != EOrderStatus.Processing)
            throw new InvalidOperationException("Can only confirm processing orders.");

        Status = EOrderStatus.Confirmed;
        ConfirmationDate = DateTime.UtcNow;
    }

    public void ShipOrder()
    {
        if (Status != EOrderStatus.Confirmed)
            throw new InvalidOperationException("Can only ship confirmed orders.");

        Status = EOrderStatus.Shipped;
    }

    public void ReceiveOrder()
    {
        if (Status != EOrderStatus.Shipped)
            throw new InvalidOperationException("Can only receive shipped orders.");

        Status = EOrderStatus.Received;
    }

    public void CancelOrder()
    {
        if (Status == EOrderStatus.Received)
            throw new InvalidOperationException("Cannot cancel received orders.");

        Status = EOrderStatus.Canceled;
    }
}