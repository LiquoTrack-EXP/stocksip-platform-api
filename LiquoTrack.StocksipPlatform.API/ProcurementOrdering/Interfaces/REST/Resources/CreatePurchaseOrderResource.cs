using System.Text.Json.Serialization;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Interfaces.REST.Resources;

/// <summary>
/// Resource for creating a purchase order.
/// </summary>
public record CreatePurchaseOrderResource
{
    /// <summary>
    /// The order code.
    /// </summary>
    public string OrderCode { get; init; }
    
    /// <summary>
    /// The catalog identifier to buy from.
    /// </summary>
    public string CatalogIdBuyFrom { get; init; }
    
    /// <summary>
    /// The buyer account identifier.
    /// </summary>
    public string Buyer { get; init; }
    
    /// <summary>
    /// The optional index of the delivery address from the account's addresses.
    /// </summary>
    public int? AddressIndex { get; init; }

    [JsonConstructor]
    public CreatePurchaseOrderResource(
        string orderCode,
        string catalogIdBuyFrom,
        string buyer,
        int? addressIndex = null)
    {
        OrderCode = orderCode;
        CatalogIdBuyFrom = catalogIdBuyFrom;
        Buyer = buyer;
        AddressIndex = addressIndex;
    }
}