using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;

public interface IOrderManagementContextFacade
{
    Task<SalesOrder?> GenerateSalesOrderAsync(GenerateSalesOrderCommand command);
    Task<IEnumerable<SalesOrder>> GetSalesOrdersByBuyerAsync(AccountId buyerId);
}
