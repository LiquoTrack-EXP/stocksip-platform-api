using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling subscription-related requests.
/// </summary>
/// <param name="subscriptionsCommandService">
///     The service for handling subscription-related commands.
/// </param>
[ApiController]
[Route("api/v1/accounts/{accountId}/subscriptions")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Accounts")]
public class AccountSubscriptionsController(
    ISubscriptionsCommandService subscriptionsCommandService,
    ISubscriptionQueryService subscriptionQueryService) : ControllerBase
{
    /// <summary>
    ///     The service for handling subscription-related commands.
    /// </summary>
    /// <param name="accountId">
    ///     The route parameter representing the unique identifier of the account for which to create a subscription.
    /// </param>
    /// <param name="resource">
    ///     The request body containing the subscription details.  
    /// </param>
    /// <returns>
    ///     The newly created subscription.
    /// </returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new subscription for an account.",
        Description = "Initializes a new subscription for the specified account.",
        OperationId = "CreateSubscription"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Subscription created successfully.", typeof(PaymentPreferenceResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Subscription could not be created.")]
    public async Task<IActionResult> CreateSubscription([FromRoute] string accountId,
        [FromBody] InitialSubscriptionResource resource)
    {
        var command = InitialSubscriptionCommandFromResourceAssembler.FromCommandToEntity(resource, accountId);
        var (preferenceId, initPoint) = await subscriptionsCommandService.Handle(command);

        if (preferenceId is null && initPoint is null)
        {
            var freeSubscription = new PaymentPreferenceResource(null, null, "Free plan activated successfully.");

            return CreatedAtAction(nameof(CreateSubscription), freeSubscription);
        }

        var subscriptionResource =
            PaymentPreferenceResourceFromEntityAssembler.ToResourceFromEntity(preferenceId!, initPoint!,
                message: "Processing subscription request..");

        return CreatedAtAction(nameof(CreateSubscription), subscriptionResource);
    }

    /// <summary>
    ///     Method to upgrade an existing subscription.
    /// </summary>
    /// <param name="accountId">
    ///     The route parameter representing the unique identifier of the account for which to create a subscription. 
    /// </param>
    /// <param name="subscriptionId">
    ///     The route parameter representing the unique identifier of the subscription to upgrade.
    /// </param>
    /// <param name="resource">
    ///     A resource containing the details for upgrading the subscription. 
    /// </param>
    /// <returns>
    ///     A 201 Created response with the newly created subscription, or a 400 Bad Request response if the subscription could not be upgraded. 
    /// </returns>
    [HttpPut("{subscriptionId}")]
    [SwaggerOperation(
        Summary = "Upgrade an existing subscription for an account.",
        Description = "Upgrades an existing subscription for the specified account.",
        OperationId = "UpdateSubscription"
        )]
    [SwaggerResponse(StatusCodes.Status201Created, "Subscription created successfully.", typeof(PaymentPreferenceResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Subscription could not be created.")]
    public async Task<IActionResult> UpdateSubscription([FromRoute] string accountId, [FromRoute] string subscriptionId,
        [FromBody] UpgradeSubscriptionResource resource)
    {
        var command =
            UpgradeSubscriptionCommandFromResourceAssembler.ToCommandFromResource(accountId, subscriptionId, resource);
        var (preferenceId, initPoint) = await subscriptionsCommandService.Handle(command);
        var subscriptionResource =
            PaymentPreferenceResourceFromEntityAssembler.ToResourceFromEntity(preferenceId!, initPoint!,
                message: "Processing subscription request..");

        return CreatedAtAction(
            nameof(CreateSubscription),
            new { accountId = accountId, subscriptionId = subscriptionId },
            subscriptionResource
        );
    }

    /// <summary>
    ///     Method to get a subscription by account ID.
    /// </summary>
    /// <param name="accountId">
    ///     The route parameter representing the unique identifier of the account for which to retrieve a subscription.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the subscription, or a 404 Not Found response if the subscription does not exist.
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get subscription by account id",
        Description = "Retrieves a subscription for the specified account.",
        OperationId = "GetSubscriptionByAccountId"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription returned successfully.", typeof(SubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Subscription not found.")]
    public async Task<IActionResult> GetSubscriptionByAccountId([FromRoute] string accountId)
    {
        var getSubscriptionByAccountIdQuery = new GetSubscriptionByAccountIdQuery(accountId);
        var (subscription, plan) = await subscriptionQueryService.Handle(getSubscriptionByAccountIdQuery);
        if (subscription is null || plan is null)
        {
            return NotFound($"Subscription with account ID {accountId} not found.");
        }
        var subscriptionResource = SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(subscription, plan);
        return Ok(subscriptionResource);
    }
}