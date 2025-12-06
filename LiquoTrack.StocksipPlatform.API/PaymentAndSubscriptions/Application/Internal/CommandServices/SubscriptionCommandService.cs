using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.PaymentProviders.
    services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.CommandServices;

public class SubscriptionCommandService(
    ISubscriptionRepository subscriptionRepository,
    IAccountRepository accountRepository,
    IPlanRepository planRepository,
    IMercadoPagoService mercadoPagoService
) : ISubscriptionsCommandService
{
    public async Task<(string?, string?)> Handle(InitialSubscriptionCommand command)
    {
        var account = await accountRepository.FindByIdAsync(command.AccountId)
                      ?? throw new Exception($"Account with ID {command.AccountId} not found.");

        var plan = await planRepository.FindByIdAsync(command.SelectedPlanId)
                   ?? throw new Exception($"Plan with ID {command.SelectedPlanId} not found.");

        var pendingSubscription =
            await subscriptionRepository.FindPendingSubscriptionByAccountIdIdAsync(command.AccountId);
        var currentSubscription =
            await subscriptionRepository.FindActiveSubscriptionByAccountIdAsync(command.AccountId);

        if (pendingSubscription is not null)
            await subscriptionRepository.DeleteAsync(pendingSubscription.Id.ToString());
        if (currentSubscription is not null)
            throw new Exception("An active subscription already exists for this account.");

        var subscription = new Subscription(command.AccountId, command.SelectedPlanId);

        string? preferenceId = null;
        string? initPoint = null;

        switch (plan.PlanType)
        {
            case EPlanType.Free:
                subscription.ActivateFreePlan(plan);
                if (account.Status == EAccountStatuses.Inactive)
                {
                    account.ActivateAccount();
                    await accountRepository.UpdateAsync(account);
                }

                break;

            case EPlanType.Premium:
            case EPlanType.Enterprise:
                var preference = mercadoPagoService.CreatePaymentPreference(
                    title: plan.Description,
                    price: plan.PlanPrice.GetAmount(),
                    currency: plan.PlanPrice.GetCurrencyCode(),
                    quantity: 1,
                    command.AccountId,
                    null,
                    null
                );
                preferenceId = preference.PreferenceId;
                initPoint = preference.InitPoint;
                subscription.MarkAsPending(plan, preferenceId);
                break;

            default:
                throw new InvalidOperationException("Unknown plan type.");
        }

        await subscriptionRepository.AddAsync(subscription);
        return (preferenceId, initPoint);
    }

    public async Task<Subscription?> Handle(WebhookPaymentCommand command)
    {
        var payment = await mercadoPagoService.GetPaymentById(command.paymentId)
                      ?? throw new Exception($"Payment with ID {command.paymentId} not found");

        if (string.IsNullOrEmpty(payment.AccountId))
            throw new Exception("Payment does not contain a valid Account ID");

        var subscription = await subscriptionRepository.FindByAccountIdAsync(payment.AccountId)
                           ?? throw new Exception("Subscription not found for this payment");

        var account = await accountRepository.FindByIdAsync(subscription.AccountId)
                      ?? throw new Exception($"Account with ID {subscription.AccountId} not found.");

        switch (payment.Status.ToLower())
        {
            case "approved":
                if (!string.IsNullOrEmpty(subscription.PendingPlanId))
                {
                    var pendingPlan = await planRepository.FindByIdAsync(subscription.PendingPlanId)
                                      ?? throw new Exception(
                                          $"Pending plan with ID {subscription.PendingPlanId} not found");

                    switch (pendingPlan.PlanType)
                    {
                        case EPlanType.Premium:
                            subscription.ActivatePremiumPlan(pendingPlan);
                            break;
                        case EPlanType.Enterprise:
                            subscription.ActivateEnterprisePlan(pendingPlan);
                            break;
                    }

                    subscription.PlanId = subscription.PendingPlanId;
                    subscription.PendingPlanId = null;
                    subscription.PreferenceId = null;
                    subscription.Status = ESubscriptionStatus.Active;
                }
                else
                {
                    var plan = await planRepository.FindByIdAsync(subscription.PlanId)
                               ?? throw new Exception($"Plan with ID {subscription.PlanId} not found");

                    switch (plan.PlanType)
                    {
                        case EPlanType.Premium:
                            subscription.ActivatePremiumPlan(plan);
                            break;
                        case EPlanType.Enterprise:
                            subscription.ActivateEnterprisePlan(plan);
                            break;
                    }
                }
                break;

            case "expired":
                if (subscription.Status == ESubscriptionStatus.PendingUpgradePayment)
                {
                    subscription.Status = ESubscriptionStatus.Active;
                    subscription.PendingPlanId = null;
                    subscription.PreferenceId = null;
                }

                break;
        }

        if (account.Status == EAccountStatuses.Inactive)
        {
            account.ActivateAccount();
            await accountRepository.UpdateAsync(account);
        }

        await subscriptionRepository.UpdateAsync(subscription);
        return subscription;
    }


    /// <summary>
    ///     Method to handle the upgrade of a subscription.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<(string?, string?)> Handle(UpgradeSubscriptionCommand command)
    {
        var currentSubscription =
            await subscriptionRepository.FindByIdAsync(command.SubscriptionId)
            ?? throw new Exception($"No active subscription found for account {command.AccountId}.");

        var newPlan = await planRepository.FindByIdAsync(command.NewPlanId)
                      ?? throw new Exception($"Plan with ID {command.NewPlanId} not found.");

        var currentPlan = await planRepository.FindByIdAsync(currentSubscription.PlanId)
                          ?? throw new Exception($"Current plan with ID {currentSubscription.PlanId} not found.");

        if (newPlan.PlanType <= currentPlan.PlanType)
            throw new InvalidOperationException("Upgrade can only be done to a higher plan.");

        var paymentPreference = mercadoPagoService.CreatePaymentPreference(
            title: $"Upgrade to {newPlan.Description}",
            price: newPlan.PlanPrice.GetAmount(),
            currency: newPlan.PlanPrice.GetCurrencyCode(),
            quantity: 1,
            command.AccountId,
            DateTime.Now,
            DateTime.Now.AddMinutes(15)
        );

        currentSubscription.MarkAsPendingUpgrade(command.NewPlanId, paymentPreference.PreferenceId);
        await subscriptionRepository.UpdateAsync(currentSubscription);

        return (paymentPreference.PreferenceId, paymentPreference.InitPoint);
    }
}