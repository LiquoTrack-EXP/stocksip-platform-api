namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources
{
    /// <summary>
    /// Represents the response for getting orders by Liquor Store Owner
    /// </summary>
    public class LiquorStoreOwnerOrdersResponse
    {
        /// <summary>
        /// The Liquor Store Owner ID
        /// </summary>
        public string LiquorStoreOwnerId { get; set; }
        
        /// <summary>
        /// The list of orders for the Liquor Store Owner
        /// </summary>
        public ICollection<SalesOrderResource> Orders { get; set; } = new List<SalesOrderResource>();
        
        /// <summary>
        /// Total number of orders
        /// </summary>
        public int TotalOrders { get; set; }
        
        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; }
    }
}
