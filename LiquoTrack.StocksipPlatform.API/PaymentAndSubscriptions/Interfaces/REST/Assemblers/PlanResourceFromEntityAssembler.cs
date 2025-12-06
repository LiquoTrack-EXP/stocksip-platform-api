using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Resources;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.REST.Assemblers;

/// <summary>
///     Static class to convert Plan entity to PlanResource.
/// </summary>
public class PlanResourceFromEntityAssembler
{
    /// <summary>
    ///     Assemble a <see cref="PlanResource"/> from a <see cref="Plan"/> entity.   
    /// </summary>
    /// <param name="entity">
    ///     The <see cref="Plan"/> entity to convert.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="PlanResource"/> representing the provided entity.
    /// </returns>
    public static PlanResource ToResourceFromEntity(Plan entity)
    {
        return new PlanResource(
            entity.Id.ToString(),
            entity.PlanType.ToString(),
            entity.Description,
            entity.PaymentFrequency.ToString(),
            entity.PlanPrice.GetAmount(),
            entity.PlanPrice.GetCurrencyCode(),
            entity.PlanLimits.MaxUsers,
            entity.PlanLimits.MaxWarehouses,
            entity.PlanLimits.MaxProducts
        );
    }
}