using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

public class PurchaseOrderItemSerializer : IBsonSerializer<PurchaseOrderItem>
{
    public Type ValueType => typeof(PurchaseOrderItem);

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, PurchaseOrderItem value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        context.Writer.WriteStartDocument();

        context.Writer.WriteName("productId");
        context.Writer.WriteString(value.ProductId.GetId);

        context.Writer.WriteName("productName");
        context.Writer.WriteString(value.ProductName);

        context.Writer.WriteName("imageUrl");
        context.Writer.WriteString(value.ImageUrl ?? string.Empty);

        context.Writer.WriteName("unitPrice");
        context.Writer.WriteDecimal128(new Decimal128(value.UnitPrice)); // <-- Convert decimal to Decimal128

        context.Writer.WriteName("quantity");
        context.Writer.WriteInt32(value.Quantity);

        context.Writer.WriteEndDocument();
    }

    public PurchaseOrderItem Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        context.Reader.ReadStartDocument();

        var productId = context.Reader.ReadString("productId");
        var productName = context.Reader.ReadString("productName");
        var imageUrl = context.Reader.ReadString("imageUrl");

        var unitPriceDecimal128 = context.Reader.ReadDecimal128("unitPrice");
        var unitPrice = (decimal)unitPriceDecimal128; // <-- cast a decimal

        var quantity = context.Reader.ReadInt32("quantity");

        context.Reader.ReadEndDocument();

        return new PurchaseOrderItem(
            new ProductId(productId),
            productName,
            imageUrl,
            unitPrice,
            quantity
        );
    }

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (PurchaseOrderItem)value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);
}