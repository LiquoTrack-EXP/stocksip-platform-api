using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Queries;

public record GetAllSalesOrdersByBuyerIdQuery(AccountId buyerId);