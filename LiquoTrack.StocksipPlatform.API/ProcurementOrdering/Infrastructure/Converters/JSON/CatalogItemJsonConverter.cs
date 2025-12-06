using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Converters.JSON;

/// <summary>
/// Custom JSON converter for CatalogItem to ensure correct serialization and deserialization.
/// </summary>
public class CatalogItemJsonConverter : JsonConverter<CatalogItem>
{
    public override CatalogItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            var root = jsonDoc.RootElement;

            var productId = new ProductId(root.GetProperty("productId").GetString()!);
            var productName = root.GetProperty("productName").GetString()!;

            var priceObj = root.GetProperty("unitPrice");
            var amount = priceObj.GetProperty("amount").GetDecimal();
            var currency = new Currency(priceObj.GetProperty("currency").GetString()!);
            var unitPrice = new Money(amount, currency);

            var imageUrl = root.GetProperty("imageUrl").GetString()!;
            var addedAt = root.GetProperty("addedAt").GetDateTime();

            return new CatalogItem(productId, productName, unitPrice, imageUrl, addedAt);
        }
    }

    public override void Write(Utf8JsonWriter writer, CatalogItem value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("productId", value.ProductId.GetId);
        writer.WriteString("productName", value.ProductName);

        writer.WriteStartObject("unitPrice");
        writer.WriteNumber("amount", value.UnitPrice.GetAmount());
        writer.WriteString("currency", value.UnitPrice.GetCurrencyCode());
        writer.WriteEndObject();

        writer.WriteString("imageUrl", value.ImageUrl);
        writer.WriteString("addedAt", value.AddedAt.ToString("o"));

        writer.WriteEndObject();
    }
}