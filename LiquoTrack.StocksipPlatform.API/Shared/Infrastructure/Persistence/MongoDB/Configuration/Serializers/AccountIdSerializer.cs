using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

/// <summary>
///     This class provides a custom serializer for the AccountId value object to be used in MongoDB.
/// </summary>
public class AccountIdSerializer : IBsonSerializer<AccountId>
{
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, AccountId value)
    {
        if (value == null)
        {
            context.Writer.WriteNull();
            return;
        }
        context.Writer.WriteString(value.GetId);
    }

    public AccountId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.GetCurrentBsonType();
        if (bsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return null;
        }
        
        var value = context.Reader.ReadString();
        return string.IsNullOrWhiteSpace(value) ? null : AccountId.Create(value);
    }

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (AccountId)value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);

    public Type ValueType => typeof(AccountId);
}