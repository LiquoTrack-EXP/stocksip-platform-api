namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;

public interface IDeliverable
{
    void ProcessOrder();
    void ConfirmOrder();
    void DeliverOrder();
    void ArriveOrder();
    void CancelOrder();
}