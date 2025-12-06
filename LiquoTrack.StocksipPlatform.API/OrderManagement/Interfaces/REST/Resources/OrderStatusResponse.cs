namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources
{
    /// <summary>
    /// Represents the response when querying an order's status
    /// </summary>
    public class OrderStatusResponse
    {
        /// <summary>
        /// The unique identifier of the order
        /// </summary>
        public string OrderId { get; set; }
        
        /// <summary>
        /// The current status of the order
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// A human-readable message about the status
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// The last update timestamp
        /// </summary>
        public DateTime LastUpdated { get; set; }
        
        /// <summary>
        /// Additional details about the status
        /// </summary>
        public string Details { get; set; }
    }
}
