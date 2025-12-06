using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Attributes;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Controllers;

/// <summary>
///     Controller for user password recovery operations.
/// </summary>
[ApiController]
[Route("api/v1/users")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Users")]
public class UserPasswordRecoveryController(
    IUserCommandService userCommandService) : ControllerBase
{
    /// <summary>
    ///     Method to send a recovery code to the email service.
    /// </summary>
    /// <param name="resource">
    ///     Resource containing the details for sending a recovery code.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the details of the recovery code sent, or a 400 Bad Request response if the recovery code could not be sent. 
    /// </returns>
    [HttpPost("recovery-code")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Send a recovery code to the email service",
        Description = "Send a recovery code to the email service",
        OperationId = "SendRecoveryCode"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Recovery code sent successfully.", typeof(object))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(string))]
    public async Task<IActionResult> RecoveryCode([FromBody] SendCodeToRecoverPasswordResource resource)
    {
        var command = SendCodeToRecoverPasswordCommandFromResourceAssembler.ToCommandFromResource(resource);
        await userCommandService.Handle(command);
        return Ok(new { Message = "Recovery code sent successfully" });
    }

    /// <summary>
    ///     Method to verify a recovery code.
    /// </summary>
    /// <param name="resource">
    ///     Resource containing the details for verifying a recovery code.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the details of the recovery code sent, or a 400 Bad Request response if the recovery code could not be sent.
    /// </returns>
    [HttpPost("verify-recovery-code")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Verify a recovery code",
        Description = "Verify a recovery code",
        OperationId = "VerifyRecoveryCode"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Recovery code verified successfully.", typeof(object))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(string))]
    public async Task<IActionResult> VerifyRecoveryCode([FromBody] VerifyRecoveryCodeResource resource)
    {
        var command = VerifyRecoveryCodeCommandFromResourceAssembler.ToCommandFromResource(resource);
        await userCommandService.Handle(command);
        return Ok(new { Message = "Recovery code verified successfully" });
    }
    
    /// <summary>
    ///     Method to reset a user password.
    /// </summary>
    /// <param name="resource">
    ///     A resource containing the details for resetting a user password.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the details of the password reset, or a 400 Bad Request response if the password could not be reset.
    /// </returns>
    [HttpPut("reset-password")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Reset user password",
        Description = "Reset user password",
        OperationId = "ResetPassword"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Password reset successfully.", typeof(object))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(string))]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordResource resource)
    {
        var command = ResetPasswordCommandFromResourceAssembler.ToCommandFromResource(resource);
        await userCommandService.Handle(command);
        return Ok(new { Message = "Password reset successfully" });
    }
}