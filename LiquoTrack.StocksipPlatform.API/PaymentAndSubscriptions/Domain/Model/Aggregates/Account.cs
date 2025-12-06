using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root that represents an Account.
/// </summary>
public class Account(
    string businessId,
    EAccountStatuses status,
    EAccountRole role
) : Entity
{
    /// <summary>
    ///     The Business Id associated with the Account.
    /// </summary>
    public string BusinessId { get; private set; } = businessId;

    /// <summary>
    ///     The status of the Account.
    /// </summary>
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public EAccountStatuses Status { get; private set; } = status;

    /// <summary>
    ///     The role of the Account.
    /// </summary>
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public EAccountRole Role { get; private set; } = role;

    /// <summary>
    ///     The subscription associated with this Account.
    ///     It can be null if the account has not yet subscribed to a plan.
    /// </summary>
    public Subscription? Subscription { get; private set; }

    /// <summary>
    ///     Backing field for the collection of addresses associated with the Account.
    ///     Initialized directly to avoid null reference when MongoDB deserializes the entity.
    /// </summary>
    [BsonElement("addresses")]
    private List<Address> _addresses = new();

    /// <summary>
    ///     The collection of addresses associated with the Account.
    /// </summary>
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    /// <summary>
    ///     Factory method to create a new Account aggregate root from a CreateAccountCommand.
    /// </summary>
    /// <param name="command">
    ///     A <see cref="CreateAccountCommand"/> used to create the Account.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="Account"/> aggregate root.
    /// </returns>
    public Account(CreateAccountCommand command)
        : this(
            command.BusinessId,
            EAccountStatuses.Inactive,
            Enum.Parse<EAccountRole>(command.AccountRole)
        )
    { }
    
    /// <summary>
    ///     Method to activate the account.
    /// </summary>
    public void ActivateAccount() => this.Status = EAccountStatuses.Active;

    /// <summary>
    ///     Method to deactivate the account.
    /// </summary>
    public void DeactivateAccount() => this.Status = EAccountStatuses.Inactive;

    /// <summary>
    ///     Method to get the creation date of the account in "yyyy-MM-dd HH:mm:ss" format.
    /// </summary>
    /// <returns>
    ///     A string representing the creation date of the account.
    /// </returns>
    public string GetCreationDate()
    {
        return this.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    ///     Adds a new address to the account.
    /// </summary>
    /// <param name="address">
    ///     The <see cref="Address"/> value object to be added to the account.
    /// </param>
    public void AddAddress(Address address)
    {
        _addresses ??= new List<Address>();
        _addresses.Add(address);
    }
}