namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

/// <summary>
///     Resource class for representing a Plan.
///     It is used for transferring data between the API and the client.
/// </summary>
public record PlanResource(
        string PlanId,
        string PlanType,
        string Description,
        string PaymentFrequency,
        decimal Price,
        string Currency,
        int MaxUsers,
        int MaxWarehouses,
        int MaxProducts
    );