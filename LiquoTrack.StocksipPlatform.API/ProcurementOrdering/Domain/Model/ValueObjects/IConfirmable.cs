namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.ValueObjects;

public interface IConfirmable
{
    void ProcessOrder();
    void ConfirmOrder();
    void ShipOrder();
    void ReceiveOrder();
    void CancelOrder();
}