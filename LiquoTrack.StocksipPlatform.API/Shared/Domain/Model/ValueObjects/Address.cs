namespace LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

/// <summary>
///     Value Object that represents a physical address.
/// </summary>
public class Address
{
    /// <summary>
    ///     The street name and number.
    /// </summary>
    public string Street { get; init; }

    /// <summary>
    ///     The city where the address is located.
    /// </summary>
    public string City { get; init; }

    /// <summary>
    ///     The state or province of the address.
    /// </summary>
    public string State { get; init; }

    /// <summary>
    ///     The country of the address.
    /// </summary>
    public string Country { get; init; }

    /// <summary>
    ///     The postal or ZIP code.
    /// </summary>
    public string ZipCode { get; init; }

    /// <summary>
    ///     Creates a new instance of the <see cref="Address"/> value object.
    /// </summary>
    public Address(string street, string city, string state, string country, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }
}