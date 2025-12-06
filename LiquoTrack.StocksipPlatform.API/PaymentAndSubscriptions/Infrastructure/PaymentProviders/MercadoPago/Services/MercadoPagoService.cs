using System.Text.Json;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders.models;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders.services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.PaymentProviders.MercadoPago.Configuration;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using Microsoft.Extensions.Options;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.PaymentProviders.MercadoPago.Services;

/// <summary>
///     MercadoPago service implementation.
/// </summary>
public class MercadoPagoService : IMercadoPagoService
{
    
    /// <summary>
    ///     MercadoPago settings.
    /// </summary>
    private readonly MercadoPagoSettings _settings;

    /// <summary>
    ///     Default constructor for MercadoPagoService.   
    /// </summary>
    /// <param name="settings">
    ///     The MercadoPago settings. 
    /// </param>
    public MercadoPagoService(IOptions<MercadoPagoSettings> settings)
    {
        _settings = settings.Value;
        MercadoPagoConfig.AccessToken = _settings.AccessToken;
    }
    
    /// <summary>
    ///     Method to create a payment preference.
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
    /// <returns>
    ///     A string representing the ID of the payment preference.
    /// </returns>
    public (string PreferenceId, string InitPoint) CreatePaymentPreference(string title, decimal price, string currency, int quantity, string accountId, DateTime? expirationDateFrom, DateTime? expirationDateTo)
    {
        var request = new PreferenceRequest
        {
            Items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Title = title,
                    Quantity = quantity,
                    CurrencyId = currency,
                    UnitPrice = price
                }
            },
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = "stocksip://payment/congrats",
                Failure = "stocksip://payment/failure",
                Pending = "stocksip://payment/pending"
            },
            NotificationUrl = "https://stocksip-back-end.azurewebsites.net/api/v1/subscriptions",
            AutoReturn = "approved",
            Metadata = new Dictionary<string, object>
            {
                {"account_id", accountId}
            },
            
            Expires = true,
            ExpirationDateFrom = expirationDateFrom,
            ExpirationDateTo = expirationDateTo
        };
        
        var client = new PreferenceClient();
        var preference = client.CreateAsync(request).Result;
        return (preference.Id, preference.InitPoint);
    }

    /// <summary>
    ///     Method to get a payment by ID.  
    /// </summary>
    /// <param name="paymentId">
    ///     The ID of the payment.
    /// </param>
    /// <returns>
    ///     A MercadoPagoPayment object representing the payment.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     A thrown exception when the payment ID is invalid.
    /// </exception>
    public async Task<MercadoPagoPayment?> GetPaymentById(string paymentId)
    {
        if (!long.TryParse(paymentId, out long id))
            throw new ArgumentException("Invalid payment ID", nameof(paymentId));

        var client = new PaymentClient();
        var payment = await client.GetAsync(id);

        string accountId = "";
        
        if (payment.Metadata is IDictionary<string, object> dict)
        {
            
            if (dict.TryGetValue("account_id", out var value))
                accountId = value?.ToString() ?? "";
        }
        else if (payment.Metadata != null)
        {
            
            try
            {
                var json = payment.Metadata.ToString();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("account_id", out var accElem))
                    accountId = accElem.GetString() ?? "";
            }
            catch
            {
                throw new Exception("Invalid payment metadata");
            }
        }

        return new MercadoPagoPayment(
            payment.Id.ToString(),
            payment.Status,
            accountId
        );
    }


    
}