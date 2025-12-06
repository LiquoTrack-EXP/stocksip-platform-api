using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services
{
    /// <summary>
    /// Service for handling low stock operations
    /// </summary>
    public interface ILowStockService
    {
        /// <summary>
        /// Get low stock items from the inventory context
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="catalogId">The catalog ID to check for low stock items</param>
        /// <returns>List of low stock items with suggested quantities</returns>
        Task<IEnumerable<LowStockItem>> GetLowStockItems(string accountId, string catalogId);
    }
}
