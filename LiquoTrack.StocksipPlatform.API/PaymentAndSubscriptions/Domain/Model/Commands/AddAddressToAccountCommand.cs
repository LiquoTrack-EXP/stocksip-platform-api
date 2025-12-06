namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

/// <summary>
/// Command to add an address to an account.
/// </summary>
/// <param name="AccountId">The account identifier.</param>
/// <param name="Street">The street address.</param>
/// <param name="City">The city.</param>
/// <param name="State">The state or province.</param>
/// <param name="Country">The country.</param>
/// <param name="ZipCode">The zip or postal code.</param>
public record AddAddressToAccountCommand(
    string AccountId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);