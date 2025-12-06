using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

/// <summary>
/// Serializer for the CatalogItem entity to be used in MongoDB.
/// </summary>
public class CatalogItemSerializer : IBsonSerializer<CatalogItem>
{
    public CatalogItem Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        reader.ReadStartDocument();

        string productId = string.Empty;
        string? productName = null;
        decimal amount = 0;
        string currency = string.Empty;
        string? imageUrl = null;
        DateTime addedAt = DateTime.UtcNow;

        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var name = reader.ReadName();
            switch (name)
            {
                case nameof(CatalogItem.ProductId):
                    productId = reader.ReadString();
                    break;

                case nameof(CatalogItem.UnitPrice):
                    reader.ReadStartDocument();
                    while (reader.ReadBsonType() != BsonType.EndOfDocument)
                    {
                        var propName = reader.ReadName();
                        switch (propName)
                        {
                            case "Amount":
                                amount = (decimal)reader.ReadDouble();
                                break;
                            case "Currency":
                                currency = reader.ReadString();
                                break;
                            default:
                                reader.SkipValue();
                                break;
                        }
                    }
                    reader.ReadEndDocument();
                    break;

                case nameof(CatalogItem.ProductName):
                    productName = reader.ReadString();
                    break;

                case nameof(CatalogItem.ImageUrl):
                    imageUrl = reader.ReadString();
                    break;

                case nameof(CatalogItem.AddedAt):
                    addedAt = BsonUtils.ToDateTimeFromMillisecondsSinceEpoch(reader.ReadDateTime());
                    break;

                default:
                    reader.SkipValue();
                    break;
            }
        }

        reader.ReadEndDocument();

        return new CatalogItem(
            new ProductId(productId),
            productName!,
            new Money(amount, new Currency(currency)),
            imageUrl!,
            addedAt
        );
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CatalogItem value)
    {
        var writer = context.Writer;
        writer.WriteStartDocument();

        writer.WriteName(nameof(CatalogItem.ProductId));
        writer.WriteString(value.ProductId.GetId);

        writer.WriteName(nameof(CatalogItem.ProductName));
        writer.WriteString(value.ProductName);

        writer.WriteName(nameof(CatalogItem.UnitPrice));
        writer.WriteStartDocument();
        writer.WriteName("Amount");
        writer.WriteDouble((double)value.UnitPrice.GetAmount());
        writer.WriteName("Currency");
        writer.WriteString(value.UnitPrice.GetCurrencyCode());
        writer.WriteEndDocument();

        writer.WriteName(nameof(CatalogItem.ImageUrl));
        writer.WriteString(value.ImageUrl);

        writer.WriteName(nameof(CatalogItem.AddedAt));
        writer.WriteDateTime(BsonUtils.ToMillisecondsSinceEpoch(value.AddedAt.ToUniversalTime()));

        writer.WriteEndDocument();
    }

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (CatalogItem)value);

    public Type ValueType => typeof(CatalogItem);
}