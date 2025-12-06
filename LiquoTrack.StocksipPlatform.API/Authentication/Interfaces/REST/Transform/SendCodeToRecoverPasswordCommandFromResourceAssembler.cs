using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;

/// <summary>
///     Static assembler class to convert SendCodeToRecoverResource to SendCodeToRecoverCommand.
/// </summary>
public class SendCodeToRecoverPasswordCommandFromResourceAssembler
{
    /// <summary>
    ///     Method to convert SendCodeToRecoverResource to SendCodeToRecoverCommand.
    /// </summary>
    /// <param name="resource">
    ///     SendCodeToRecoverResource to convert.
    /// </param>
    /// <returns>
    ///     A new SendCodeToRecoverCommand.
    /// </returns>
    public static SendCodeToRecoverPasswordCommand ToCommandFromResource(SendCodeToRecoverPasswordResource resource)
        => new SendCodeToRecoverPasswordCommand(resource.Email);
}