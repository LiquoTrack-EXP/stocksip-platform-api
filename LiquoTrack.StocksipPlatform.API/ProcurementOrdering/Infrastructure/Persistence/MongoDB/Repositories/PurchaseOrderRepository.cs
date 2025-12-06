using Cortex.Mediator;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Repositories;
using MongoDB.Driver;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Persistence.MongoDB.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="PurchaseOrder"/> entities.
/// </summary>
public class PurchaseOrderRepository : BaseRepository<PurchaseOrder>, IPurchaseOrderRepository
{
    private readonly IMongoCollection<PurchaseOrder> _orderCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="PurchaseOrderRepository"/> class.
    /// </summary>
    /// <param name="context">The MongoDB database context.</param>
    /// <param name="mediator">The domain event mediator.</param>
    public PurchaseOrderRepository(AppDbContext context, IMediator mediator)
        : base(context, mediator)
    {
        _orderCollection = context.GetCollection<PurchaseOrder>()
            ?? throw new ArgumentNullException(nameof(context), "The MongoDB context cannot be null.");
    }
    
    public async Task<PurchaseOrder?> GetByIdAsync(PurchaseOrderId id)
    {
        if (string.IsNullOrWhiteSpace(id.GetId))
            throw new ArgumentException("PurchaseOrderId cannot be null or empty.", nameof(id));

        return await _orderCollection
            .Find(order => order.Id.ToString() == id.GetId)
            .FirstOrDefaultAsync();
    }
    
    public async Task<PurchaseOrder?> GetByOrderCodeAsync(string orderCode)
    {
        ValidateNonEmpty(orderCode, nameof(orderCode));

        return await _orderCollection
            .Find(order => order.OrderCode == orderCode)
            .FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<PurchaseOrder>> FindByBuyerAsync(AccountId buyer)
    {
        if (string.IsNullOrWhiteSpace(buyer.GetId))
            throw new ArgumentException("Buyer AccountId cannot be null or empty.", nameof(buyer));

        var filter = Builders<PurchaseOrder>.Filter.Eq("buyer", buyer.GetId);
        return await _orderCollection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<PurchaseOrder>> FindByCatalogAsync(CatalogId catalogId)
    {
        if (string.IsNullOrWhiteSpace(catalogId.GetId()))
            throw new ArgumentException("CatalogId cannot be null or empty.", nameof(catalogId));

        return await _orderCollection
            .Find(order => order.CatalogIdBuyFrom.GetId() == catalogId.GetId())
            .ToListAsync();
    }
    
    public async Task<bool> ExistsByOrderCodeAsync(string orderCode)
    {
        ValidateNonEmpty(orderCode, nameof(orderCode));

        return await _orderCollection
            .Find(order => order.OrderCode == orderCode)
            .AnyAsync();
    }

    /// <summary>
    /// Validates that a string parameter is not null, empty, or whitespace.
    /// </summary>
    private static void ValidateNonEmpty(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);
    }
}