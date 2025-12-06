using System.ComponentModel.DataAnnotations;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;

public class ProcurementOrderCompletedResource
{
    [Required]
    public string OrderCode { get; set; } = default!;
    [Required]
    public string PurchaseOrderId { get; set; } = default!;
    [Required]
    public string CatalogIdBuyFrom { get; set; } = default!;
    [Required]
    public string BuyerAccountId { get; set; } = default!;
    public DateTime? ReceiptDate { get; set; }
    public DateTime? CompletitionDate { get; set; }
    [MinLength(1)]
    public List<ProcurementOrderCompletedItemResource> Items { get; set; } = new();
}
