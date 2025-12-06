using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Events;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;

/// <summary>
///     This abstract class represents the base entity for all entities in the system.
///     It contains the common properties that all entities share.
/// </summary>
public abstract class Entity
{
    /// <summary>
    ///     Id of the generic entity.
    /// </summary>
    /// 
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    
    /// <summary>
    ///     The date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    ///     The date and time when the entity was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    ///     The list of events related to the entity
    /// </summary>
    [BsonIgnore]
    private List<IDomainEvent> _domainEvents;
    
    /// <summary>
    /// Constructor to initialize the entity.
    /// </summary>
    protected Entity()
    {
        _domainEvents = [];
    }
    
    /// <summary>
    ///     The read-only collection of domain events associated with the entity.
    /// </summary>
    [BsonIgnore]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => (_domainEvents ??= []).AsReadOnly();

    
    /// <summary>
    ///     Method to add a domain event to the entity.
    /// </summary>
    /// <param name="eventItem">
    ///     The domain event to add.
    /// </param>
    protected void AddDomainEvent(IDomainEvent eventItem)
    {
        if (eventItem == null) return;
        (_domainEvents ??= []).Add(eventItem);
    }

    /// <summary>
    ///     Method to clear the domain events associated with the entity.
    /// </summary>
    public void ClearDomainEvents()
    {
        (_domainEvents ??= []).Clear();
    }
}