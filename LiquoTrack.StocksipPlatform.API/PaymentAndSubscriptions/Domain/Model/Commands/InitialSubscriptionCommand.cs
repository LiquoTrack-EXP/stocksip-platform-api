namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

/// <summary>
///     Command to activate a paid subscription for a given account and plan.
/// </summary>
/// <param name="AccountId">
///     The unique identifier of the account for which the paid subscription is to be activated.
/// </param>
/// <param name="SelectedPlanId">
///     The unique identifier of the plan to be activated.
/// </param>
public record InitialSubscriptionCommand(string AccountId, string SelectedPlanId);