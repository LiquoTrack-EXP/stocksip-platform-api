using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;

/// <summary>
///     Static assembler class to convert VerifyRecoveryCodeResource to VerifyRecoveryCodeCommand.
/// </summary>
public class VerifyRecoveryCodeCommandFromResourceAssembler
{
    /// <summary>
    ///     Method to convert VerifyRecoveryCodeResource to VerifyRecoveryCodeCommand.
    /// </summary>
    /// <param name="resource">
    ///     Resource to convert.
    /// </param>
    /// <returns>
    ///     A new VerifyRecoveryCodeCommand.
    /// </returns>
    public static VerifyRecoveryCodeCommand ToCommandFromResource(VerifyRecoveryCodeResource resource)
        => new VerifyRecoveryCodeCommand(resource.Email, resource.Code);
}