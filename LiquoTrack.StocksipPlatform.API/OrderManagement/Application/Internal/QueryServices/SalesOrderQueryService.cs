using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Application.Internal.QueryServices
{
    /// <summary>
    /// Sales Order Query Service
    /// </summary>
    public class SalesOrderQueryService(ISalesOrderRepository salesOrderRepository) : ISalesOrderQueryService
    {
        /// <summary>
        /// Handle the get all sales orders by buyer id query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SalesOrder>> Handle(GetAllSalesOrdersByBuyerIdQuery query)
        {
            return await salesOrderRepository.GetAllSalesOrdersByBuyerId(query.buyerId);
        }

        /// <summary>
        /// Handle the get all sales orders query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SalesOrder>> Handle(GetAllSalesOrdersQuery query)
        {
            return await salesOrderRepository.GetAllSalesOrders();
        }

        public async Task<IEnumerable<SalesOrder>> Handle(GetAllSalesOrdersBySupplierIdQuery query)
        {
            return await salesOrderRepository.GetAllSalesOrdersBySupplierId(query.supplierId);
        }
    }
}
