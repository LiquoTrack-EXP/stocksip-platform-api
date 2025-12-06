using MongoDB.Bson.Serialization.Attributes;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using CatalogId = LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.CatalogId;
using ESalesOrderStatuses = LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects.ESalesOrderStatuses;
using PurchaseOrderId = LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects.PurchaseOrderId;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;

public class SalesOrder : Entity
{
    private List<StatusHistory> _statusHistory;

    public string OrderCode { get; private set; }
    public PurchaseOrderId PurchaseOrderId { get; private set; }
    public ICollection<SalesOrderItem> Items { get; private set; }
    public ESalesOrderStatuses Status { get; private set; }
    public CatalogId CatalogToBuyFrom { get; private set; }
    public DateTime ReceiptDate { get; private set; }
    public DateTime CompletitionDate { get; private set; }
    public AccountId AccountId { get; private set; }

    [BsonIgnoreIfNull]
    public AccountId? SupplierId { get; private set; }

    public IReadOnlyCollection<StatusHistory> StatusHistory => (_statusHistory ??= new List<StatusHistory>()).AsReadOnly();
    public DeliveryProposal? DeliveryProposal { get; private set; }

    public SalesOrder(string orderCode, PurchaseOrderId purchaseOrderId, ICollection<SalesOrderItem> items,
                      ESalesOrderStatuses status, CatalogId catalogToBuyFrom, DateTime receiptDate,
                      DateTime completitionDate, AccountId accountId)
    {
        OrderCode = orderCode ?? throw new ArgumentNullException(nameof(orderCode));
        PurchaseOrderId = purchaseOrderId ?? throw new ArgumentNullException(nameof(purchaseOrderId));
        Items = items ?? new List<SalesOrderItem>();
        Status = status;
        CatalogToBuyFrom = catalogToBuyFrom;
        ReceiptDate = receiptDate;
        CompletitionDate = completitionDate;
        AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        _statusHistory = new List<StatusHistory> { new(Status, "System", "Order created") };
    }

    public void SetSupplier(AccountId supplierId)
    {
        SupplierId = supplierId ?? throw new ArgumentNullException(nameof(supplierId));
    }

    public void AddItem(ProductId productId, Money unitPrice, int quantityToSell, string productName)
    {
        if (productId == null) throw new ArgumentNullException(nameof(productId));
        if (unitPrice == null) throw new ArgumentNullException(nameof(unitPrice));
        if (quantityToSell <= 0) throw new ArgumentException("Quantity must be greater than zero", nameof(quantityToSell));

        Items.Add(new SalesOrderItem(productId, unitPrice, quantityToSell, productName));
    }

    public void RemoveItem(ProductId productId)
    {
        var item = Items.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
            Items.Remove(item);
    }

    public Money CalculateTotal()
    {
        if (Items == null || !Items.Any())
            return new Money(0, new Currency(nameof(EValidCurrencyCodes.USD)));

        return Items.Aggregate(new Money(0, new Currency(nameof(EValidCurrencyCodes.USD))),
            (total, item) => total.Add(item.CalculateSubTotal()));
    }

    public void UpdateStatus(ESalesOrderStatuses newStatus, string? reason = null, string changedBy = "System")
    {
        if (Status == newStatus) return;

        _statusHistory ??= new List<StatusHistory>();
        _statusHistory.Add(new StatusHistory(Status, changedBy, reason));
        Status = newStatus;

        var now = DateTime.UtcNow;
        switch (newStatus)
        {
            case ESalesOrderStatuses.Received:
                ReceiptDate = now;
                break;
            case ESalesOrderStatuses.Confirmed:
            case ESalesOrderStatuses.Arrived:
                CompletitionDate = now;
                break;
        }
    }

    public StatusHistory GetCurrentStatusInfo()
    {
        return _statusHistory?.OrderByDescending(sh => sh.Timestamp).FirstOrDefault()
               ?? new StatusHistory(Status, "System", "Initial status");
    }

    public void ProposeDelivery(DateTime proposedDate, string? notes)
    {
        DeliveryProposal = new DeliveryProposal
        {
            ProposedDate = proposedDate,
            Notes = notes,
            Status = DeliveryProposalStatus.Proposed,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AcceptDeliveryProposal(string? notes = null)
    {
        if (DeliveryProposal?.Status != DeliveryProposalStatus.Proposed)
            throw new InvalidOperationException("No pending delivery proposal to accept");

        DeliveryProposal.Status = DeliveryProposalStatus.Accepted;
        DeliveryProposal.RespondedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(notes)) DeliveryProposal.Notes = notes;
    }

    public void RejectDeliveryProposal(string? notes = null)
    {
        if (DeliveryProposal?.Status != DeliveryProposalStatus.Proposed)
            throw new InvalidOperationException("No pending delivery proposal to reject");

        DeliveryProposal.Status = DeliveryProposalStatus.Rejected;
        DeliveryProposal.RespondedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(notes)) DeliveryProposal.Notes = notes;
    }
}
