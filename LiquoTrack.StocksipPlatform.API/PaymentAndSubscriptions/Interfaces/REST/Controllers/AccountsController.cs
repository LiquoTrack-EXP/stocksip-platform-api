using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Controllers;

/// <summary>
///     Controller for handling account-related requests.
/// </summary>
/// <param name="accountQueryService">
///     The service for handling account-related queries.
/// </param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available endpoints for accounts.")]
public class AccountsController(IAccountQueryService accountQueryService, IBusinessQueryService businessQueryService) : ControllerBase
{
    /// <summary>
    ///     Endpoint to handle the retrieval of an account by its ID.
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account to retrieve.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the details of the account, or a 404 Not Found response if the account does not exist.
    /// </returns>
    [HttpGet("{accountId}")]
    [SwaggerOperation(
        Summary = "Get Account By ID",
        Description = "Retrieves an account by its ID.",
        OperationId = "GetAccountById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Account returned successfully.", typeof(AccountResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Account not found.")]
    public async Task<IActionResult> GetAccountById([FromRoute] string accountId)
    {
        var account = await accountQueryService.Handle(new GetAccountByIdQuery(accountId));
        if (account is null) return NotFound($"Account with ID {accountId} not found.");
        // Enrich with Business email if available
        string? email = null;
        try
        {
            var business = await businessQueryService.Handle(new GetBusinessByAccountIdQuery(accountId));
            email = business?.BusinessEmail?.Value;
        }
        catch
        {
            // If business is not found or any error occurs, we proceed without email
        }
        var resource = AccountResourceFromEntityAssembler.ToResourceFromEntity(account, email, null);
        return Ok(resource);
    }
    
    /// <summary>
    ///     Method to handle the retrieval of an account's status by its ID.'
    /// </summary>
    /// <param name="accountId">
    ///     The ID of the account to retrieve.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the status of the account, or a 404 Not Found response if the account does not exist.   
    /// </returns>
    [HttpGet("{accountId}/status")]
    [SwaggerOperation(
        Summary = "Get Account Status By ID",
        Description = "Retrieves the status of an account by its ID.",
        OperationId = "GetAccountStatusById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Account status returned successfully.", typeof(AccountStatusResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Account not found."
        )]
    public async Task<IActionResult> GetAccountStatusById([FromRoute] string accountId)
    {
        var status = await accountQueryService.Handle(new GetAccountStatusByIdQuery(accountId));
        if (status is null) return NotFound($"Account with ID {accountId} not found.");
        var resource = AccountStatusResourceFromEntityAssembler.ToResourceFromEntity(status);
        return Ok(resource);
    }
    
    /// <summary>
    ///     Endpoint to retrieve all accounts filtered by role.
    /// </summary>
    /// <param name="role">
    ///     Optional role to filter accounts, e.g., "supplier".
    /// </param>
    /// <returns>
    ///     A 200 OK response with the list of accounts matching the specified role,
    ///     or a 404 Not Found response if no accounts match.
    /// </returns>
    [HttpGet("roles")]
    [SwaggerOperation(
        Summary = "Get Accounts By Role",
        Description = "Retrieves all accounts with the specified role.",
        OperationId = "GetAccountsByRole")]
    [SwaggerResponse(StatusCodes.Status200OK, "Accounts returned successfully.", typeof(IEnumerable<AccountResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No accounts found for the specified role.")]
    public async Task<IActionResult> GetAccountsByRole([FromQuery] string? role)
    {
        var accounts = await accountQueryService.Handle(new GetAccountsByRoleQuery(role));

        if (accounts is null || !accounts.Any())
        {
            return NotFound($"No accounts found with role '{role}'.");
        }

        var resources = accounts.Select(account =>
            AccountResourceFromEntityAssembler.ToResourceFromEntity(
                account,
                null,
                null
            ));

        return Ok(resources);
    }

}