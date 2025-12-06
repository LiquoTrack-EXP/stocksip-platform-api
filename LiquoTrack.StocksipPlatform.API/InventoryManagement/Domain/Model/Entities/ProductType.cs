using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Entities;

/// <summary>
///     Class for representing a product type in inventory.
/// </summary>
public class ProductType : Entity
{
    /// <summary>
    ///     The name of the product type.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public EProductTypes Name { get; private set; }
    
    public ProductType() { }
    
    public ProductType(EProductTypes name)
    {
        Name = name;
    }
}