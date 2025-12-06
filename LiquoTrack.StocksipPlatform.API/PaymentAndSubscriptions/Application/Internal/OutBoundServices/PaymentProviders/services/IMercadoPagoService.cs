using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders.models;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders.services;

/// <summary>
///     Interface for MercadoPago service.
/// </summary>
public interface IMercadoPagoService
{
    
    /// <summary>
    ///     Method to create a payment preference in MercadoPago.
    /// </summary>
    /// <param name="title">
    ///     The title of the payment preference.
    /// </param>
    /// <param name="price">
    ///     The price of the payment preference.
    /// </param>
    /// <param name="currency">
    ///     The currency of the payment preference.
    /// </param>
    /// <param name="quantity">
    ///     The quantity of the payment preference.
    /// </param>
    /// <param name="accountId">
    ///     The ID of the account.
    /// </param>
    /// <param name="expirationDateFrom">
    ///     The expiration date from.
    /// </param>
    /// <param name="expirationDateTo">
    ///     The expiration date to.
    /// </param>
    /// <returns>
    ///     A string representing the ID of the payment preference.
    /// </returns>
    (string PreferenceId, string InitPoint) CreatePaymentPreference(string title, decimal price, string currency, int quantity, string accountId, DateTime? expirationDateFrom, DateTime? expirationDateTo);
    
    /// <summary>
    ///     Method to get a payment by its ID.
    /// </summary>
    /// <param name="paymentId">
    ///     The ID of the payment.
    /// </param>
    /// <returns>
    ///     A MercadoPagoPayment object representing the payment.
    /// </returns>
    Task<MercadoPagoPayment?> GetPaymentById(string paymentId);
}