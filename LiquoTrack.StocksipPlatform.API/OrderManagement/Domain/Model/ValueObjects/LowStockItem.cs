namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects
{
    public class LowStockItem
    {
        public string ProductId { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int SuggestedQuantity { get; set; }

        public string ProductName { get; set; }
    }
}
