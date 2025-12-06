namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;

public class ProcurementOrderCompletedItemResource
{
    public string ProductId { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public string ProductName { get; set; } = default!;
}
