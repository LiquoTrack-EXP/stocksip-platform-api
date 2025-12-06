using System.Collections.Generic;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources
{
    /// <summary>
    /// Represents the response for getting orders by supplier
    /// </summary>
    public class SupplierOrdersResponse
    {
        /// <summary>
        /// The supplier ID
        /// </summary>
        public string SupplierId { get; set; }
        
        /// <summary>
        /// The list of orders for the supplier
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
