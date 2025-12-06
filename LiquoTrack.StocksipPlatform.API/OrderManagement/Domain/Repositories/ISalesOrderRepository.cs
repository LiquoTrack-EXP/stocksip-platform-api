using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Repositories;

/// <summary>
///     Repository interface for managing SalesOrder entities.
/// </summary>
public interface ISalesOrderRepository : IBaseRepository<SalesOrder>
{
    Task<SalesOrder> GenerateSalesOrder(GenerateSalesOrderCommand command);
    Task<IEnumerable<SalesOrder>> GetAllSalesOrdersByBuyerId(AccountId buyerId);
    Task<IEnumerable<SalesOrder>> GetAllSalesOrdersBySupplierId(AccountId supplierId);
    Task<IEnumerable<SalesOrder>> GetAllSalesOrders();
    Task<SalesOrder> GetByIdAsync(string id);
}