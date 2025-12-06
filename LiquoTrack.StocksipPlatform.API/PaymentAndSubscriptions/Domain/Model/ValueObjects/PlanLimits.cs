namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing the limits associated with a subscription plan.
/// </summary>
public record PlanLimits
{
    /// <summary>
    ///     The maximum number of users allowed under the plan.
    /// </summary>
    public int MaxUsers { get; init; }
    
    /// <summary>
    ///     The maximum number of warehouses allowed under the plan.
    /// </summary>
    public int MaxWarehouses { get; init; }
    
    /// <summary>
    ///     The maximum number of products allowed under the plan.
    /// </summary>
    public int MaxProducts { get; init; }

    /// <summary>
    ///     Default constructor for PlanLimits.
    /// </summary>
    /// <param name="maxUsers">
    ///     The maximum number of users allowed under the plan.
    /// </param>
    /// <param name="maxWarehouses">
    ///     The maximum number of warehouses allowed under the plan.
    /// </param>
    /// <param name="maxProducts">
    ///     The maximum number of products allowed under the plan.
    /// </param>
    public PlanLimits(int maxUsers, int maxWarehouses, int maxProducts)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxProducts);
        ArgumentOutOfRangeException.ThrowIfNegative(maxUsers);
        ArgumentOutOfRangeException.ThrowIfNegative(maxWarehouses);
        
        MaxProducts = maxProducts;
        MaxUsers = maxUsers;
        MaxWarehouses = maxWarehouses;
    }

    /// <summary>
    ///     Method to get the plan limits based on the provided plan type.
    /// </summary>
    /// <param name="planType">
    ///     The type of subscription plan.
    /// </param>
    /// <returns>
    ///     A <see cref="PlanLimits"/> object containing the limits for the specified plan type.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     An exception thrown when an unsupported plan type is provided.
    /// </exception>
    public static PlanLimits For(EPlanType planType) => planType switch
    {
        EPlanType.Free => new PlanLimits(10, 5, 500),
        EPlanType.Premium => new PlanLimits(100, 20, 2000),
        EPlanType.Enterprise => new PlanLimits(200, 40, 20000),
        _ => throw new ArgumentOutOfRangeException(nameof(planType), $"Unsupported plan type: {planType}")
    };
}