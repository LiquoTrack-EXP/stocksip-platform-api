using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.ACL;

public interface IProcurementOrderingFacade
{
    Task<PurchaseOrderResource> GetPurchaseOrderResourceAsync(string purchaseOrderId);
}