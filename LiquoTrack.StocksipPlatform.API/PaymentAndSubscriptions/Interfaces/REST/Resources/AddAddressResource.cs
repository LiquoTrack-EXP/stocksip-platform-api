namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

/// <summary>
/// Resource for adding an address to an account.
/// </summary>
/// <param name="street">The street address.</param>
/// <param name="city">The city.</param>
/// <param name="state">The state or province.</param>
/// <param name="country">The country.</param>
/// <param name="zipCode">The zip or postal code.</param>
public record AddAddressResource(
    string street,
    string city,
    string state,
    string country,
    string zipCode
);