using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

/// <summary>
///     Static class to convert AccountStatus entity to AccountStatusResource.
/// </summary>
public class AccountStatusResourceFromEntityAssembler
{
    /// <summary>
    ///     Method to convert AccountStatus entity to AccountStatusResource. 
    /// </summary>
    /// <param name="accountStatus">
    ///     The AccountStatus entity to convert.
    /// </param>
    /// <returns>
    ///     A new instance of AccountStatusResource representing the provided entity.
    /// </returns>
    public static AccountStatusResource ToResourceFromEntity(string accountStatus)
    {
        return new AccountStatusResource(accountStatus);
    }
}