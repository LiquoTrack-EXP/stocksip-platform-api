using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

/// <summary>
///     Static class to convert Account entity to AccountResource.
/// </summary>
public class AccountResourceFromEntityAssembler
{
    /// <summary>
    ///     Method to convert Account entity to AccountResource.  
    /// </summary>
    /// <param name="entity">
    ///     The Account entity to convert. 
    /// </param>
    /// <returns>
    ///     A new instance of AccountResource representing the provided entity.
    /// </returns>
    public static AccountResource ToResourceFromEntity(Account entity)
    {
        return new AccountResource(
            entity.Id.ToString(),
            entity.BusinessId,
            entity.Status.ToString(),
            entity.Role.ToString(),
            entity.GetCreationDate(),
            null,
            null
        );
    }

    /// <summary>
    ///     Overload to convert Account entity to AccountResource with contact details.
    /// </summary>
    /// <param name="entity">The Account entity to convert.</param>
    /// <param name="email">The contact email to include.</param>
    /// <param name="phone">The contact phone to include.</param>
    /// <returns>An AccountResource enriched with contact details.</returns>
    public static AccountResource ToResourceFromEntity(Account entity, string? email, string? phone)
    {
        return new AccountResource(
            entity.Id.ToString(),
            entity.BusinessId,
            entity.Status.ToString(),
            entity.Role.ToString(),
            entity.GetCreationDate(),
            email,
            phone
        );
    }
}