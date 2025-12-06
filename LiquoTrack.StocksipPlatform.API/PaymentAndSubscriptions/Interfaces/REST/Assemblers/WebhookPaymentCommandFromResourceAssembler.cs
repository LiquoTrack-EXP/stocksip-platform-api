using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

public class WebhookPaymentCommandFromResourceAssembler
{
    public static WebhookPaymentCommand ToCommandFromResource(string paymentId)
    {
        return new WebhookPaymentCommand(paymentId);
    }   
}