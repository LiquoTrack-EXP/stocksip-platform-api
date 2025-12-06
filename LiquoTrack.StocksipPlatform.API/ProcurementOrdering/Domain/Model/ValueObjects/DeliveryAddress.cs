

using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.ValueObjects;

/// <summary>
///     Value Object that represents a delivery address for purchase orders.
/// </summary>
public class DeliveryAddress
{
    public string Street { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string Country { get; init; }
    public string ZipCode { get; init; }

    public DeliveryAddress(string street, string city, string state, string country, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }
    
    /// <summary>
    ///     Creates a DeliveryAddress from an Address from PaymentAndSubscriptions context.
    /// </summary>
    public static DeliveryAddress FromAddress(Address address)
    {
        return new DeliveryAddress(
            address.Street,
            address.City,
            address.State,
            address.Country,
            address.ZipCode
        );
    }
}