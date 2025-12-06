namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

/// <summary>
///     Resource class for representing an Account.
///     It is used for transferring data between the API and the client. 
/// </summary>
public record AccountResource(
        string Id,
        string BusinessId,
        string Status,
        string Role,
        string CreationDate,
        string? Email,
        string? Phone
    );