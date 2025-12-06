using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using MongoDB.Bson.Serialization;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

/// <summary>
///     Serializer for the ProductExpirationDate value object to be used in MongoDB.
/// </summary>
public class ProductExpirationDateSerializer : IBsonSerializer<ProductExpirationDate>
{
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ProductExpirationDate value)
    {
        if (value == null)
        {
            context.Writer.WriteNull();
            return;
        }

        var date = value.GetValue();
        var dateTime = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var milliseconds = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        context.Writer.WriteDateTime(milliseconds);
    }


    public ProductExpirationDate Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var milliseconds = context.Reader.ReadDateTime();
        var dateOffSet = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        var value = DateOnly.FromDateTime(dateOffSet.UtcDateTime);
        return new ProductExpirationDate(value);
    }

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (ProductExpirationDate)value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);
    
    public Type ValueType => typeof(ProductExpirationDate);
}