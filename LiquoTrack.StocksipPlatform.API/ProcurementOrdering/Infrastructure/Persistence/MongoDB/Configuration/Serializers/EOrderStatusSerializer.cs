using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

public class EOrderStatusSerializer : IBsonSerializer<EOrderStatus>
{
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, EOrderStatus value)
    {
        context.Writer.WriteString(value.ToString());
    }

    public EOrderStatus Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var enumString = context.Reader.ReadString();
        return Enum.Parse<EOrderStatus>(enumString);
    }

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (EOrderStatus)value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);

    public Type ValueType => typeof(EOrderStatus);
}