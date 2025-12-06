using System.Net.Mime;
using System.Text.Json;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Attributes;
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
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Subscriptions")]
public class SubscriptionsController(
    ISubscriptionsCommandService subscriptionsCommandService,
    ISubscriptionQueryService subscriptionQueryService) : ControllerBase
{
    /// <summary>
    ///     Method to handle the confirmation of a subscription.
    /// </summary>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    [HttpPost]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Creates a new subscription",
        Description = "Endpoint to creates a subscription based on the provided notification from MercadoPago.",
        OperationId = "ConfirmedSubscription")]
    public async Task<IActionResult> ConfirmedSubscription([FromServices] ILogger<SubscriptionsController> logger)
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            logger.LogInformation("Received Webhook: {Body}", body);
            
            _ = Task.Run(async () =>
            {
                try
                {
                    using var doc = JsonDocument.Parse(body);
                    var root = doc.RootElement;

                    string? dataId = null;
                    if (root.TryGetProperty("data", out var dataElem) && dataElem.TryGetProperty("id", out var idElem))
                        dataId = idElem.GetString();
                    else if (root.TryGetProperty("resource", out var resourceElem))
                        dataId = resourceElem.GetString()?.Split('/').Last();

                    if (string.IsNullOrWhiteSpace(dataId))
                    {
                        logger.LogWarning("Could not get payment ID from webhook");
                        return;
                    }

                    logger.LogInformation("Processing payment with ID: {DataId}", dataId);

                    var webHookCommand = WebhookPaymentCommandFromResourceAssembler.ToCommandFromResource(dataId);
                    await subscriptionsCommandService.Handle(webHookCommand);

                    logger.LogInformation("Subscription updated successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing webhook");
                }
            });

            return Ok(new { message = "Webhook received successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing webhook");
            return Ok(new { message = "Webhook received with errors (but confirmed)" });
        }
    }
}
