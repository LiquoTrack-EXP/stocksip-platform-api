using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson.Serialization;

namespace LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

/// <summary>
///     This class provides a custom serializer for the AccountId value object to be used in MongoDB.
/// </summary>
public class AccountIdSerializer : IBsonSerializer<AccountId>
{
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, AccountId value)
    {
        context.Writer.WriteString(value.GetId);
    }

    public AccountId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var value = context.Reader.ReadString();
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException("AccountId cannot be null or empty");
        }
        return AccountId.Create(value);
    }

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (AccountId)value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);

    public Type ValueType => typeof(AccountId);
}