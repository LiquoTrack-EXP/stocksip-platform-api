using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
///     Repository implementation for Plans entity.
/// </summary>
public class PlanRepository(AppDbContext context, IMediator mediator) : BaseRepository<Plan>(context, mediator), IPlanRepository
{

    /// <summary>
    ///     The MongoDB collection for the Plan entity.
    /// </summary>
    private readonly IMongoCollection<Plan> _planCollection = context.GetCollection<Plan>();
    
    public async Task SeedPlansAsync()
    {
        var defaultPlans = new List<Plan>()
        {
            new Plan(
                EPlanType.Free,
                "Free plan with limited features",
                EPaymentFrequency.None,
                new Money(0m, new Currency("USD")),
                PlanLimits.For(EPlanType.Free)
            ),
            new Plan(
                EPlanType.Premium,
                "Premium plan with more features",
                EPaymentFrequency.Monthly,
                new Money(5.99m, new Currency("USD")),
                PlanLimits.For(EPlanType.Premium)
            ),
            new Plan(
                EPlanType.Enterprise,
                "Enterprise plan with unlimited features",
                EPaymentFrequency.Yearly,
                new Money(42.99m, new Currency("USD")),
                PlanLimits.For(EPlanType.Enterprise)
            )
        };
        
        var existingPlansTypes = await _planCollection
            .Find(FilterDefinition<Plan>.Empty)
            .Project(p => p.PlanType)
            .ToListAsync();
        
        var plansToAdd = defaultPlans
            .Where(plan => !existingPlansTypes.Contains(plan.PlanType))
            .ToList();
        
        if (plansToAdd.Count != 0) await _planCollection.InsertManyAsync(plansToAdd);
    }

    /// <summary>
    ///     Method to find the maximum warehouse limit for a plan by its ID.
    /// </summary>
    /// <param name="planId">
    ///     The ID of the plan.
    /// </param>
    /// <returns>
    ///     An integer representing the maximum warehouse limit.
    /// </returns>
    public async Task<int?> FindPlanWarehouseLimitsByAccountIdAsync(string planId)
    {
        var plan = await _planCollection
            .Find(p => p.Id.ToString() == planId)
            .FirstOrDefaultAsync();

        return plan?.PlanLimits?.MaxWarehouses;
    }

    /// <summary>
    ///     Method to find the maximum products limit for a plan by its ID.   
    /// </summary>
    /// <param name="planId">
    ///     The ID of the plan.
    /// </param>
    /// <returns>
    ///     An integer representing the maximum products limit. 
    /// </returns>
    public async Task<int?> FindPlanProductsLimitByAccountIdAsync(string planId)
    {
        var plan = await _planCollection
            .Find(p => p.Id.ToString() == planId)
            .FirstOrDefaultAsync();

        return plan?.PlanLimits?.MaxProducts;
    }

    /// <summary>
    ///     Method to find the maximum users limit for a plan by its ID.  
    /// </summary>
    /// <param name="planId">
    ///     The ID of the plan.
    /// </param>
    /// <returns>
    ///     An integer representing the maximum users limit.
    /// </returns>
    public async Task<int?> FindPlanUsersLimitByAccountIdAsync(string planId)
    {
        var plan = await _planCollection
            .Find(p => p.Id.ToString() == planId)
            .FirstOrDefaultAsync();
        
        return plan?.PlanLimits?.MaxUsers;
    }
}