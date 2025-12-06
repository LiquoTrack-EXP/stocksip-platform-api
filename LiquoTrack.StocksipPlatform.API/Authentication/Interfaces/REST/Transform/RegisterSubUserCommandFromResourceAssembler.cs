using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;

/// <summary>
///     Static assembler class to convert RegisterSubUserResource to RegisterSubUserCommand.
/// </summary>
public class RegisterSubUserCommandFromResourceAssembler
{
    /// <summary>
    ///     Method to convert RegisterSubUserResource to RegisterSubUserCommand.
    /// </summary>
    /// <param name="resource">
    ///     The RegisterSubUserResource to convert.
    /// </param>
    /// <param name="accountId">
    ///     The ID of the account the user belongs to.
    /// </param>
    /// <returns>
    ///     A new RegisterSubUserCommand.
    /// </returns>
    public static RegisterSubUserCommand ToCommandFromResource(RegisterSubUserResource resource, string accountId)
    {
        return new RegisterSubUserCommand(
            resource.Name,
            resource.Email,
            resource.Password,
            resource.Role,
            resource.PhoneNumber,
            accountId,
            resource.ProfileRole
        );
    }
}