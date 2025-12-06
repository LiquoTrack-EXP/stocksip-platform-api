using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Infrastructure.Converters.JSON;

public class SalesOrderJsonConverter : JsonConverter<SalesOrder>
{
    public override SalesOrder? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;

        var idValue = root.TryGetProperty("id", out var idElement) && idElement.ValueKind == JsonValueKind.String
            ? idElement.GetString()
            : null;

        var orderCode = root.GetProperty("orderCode").GetString()
            ?? throw new JsonException("orderCode is required");

        var purchaseOrderIdValue = root.GetProperty("purchaseOrderId").GetString()
            ?? throw new JsonException("purchaseOrderId is required");

        if (!Enum.TryParse(root.GetProperty("status").GetString(), true, out ESalesOrderStatuses status))
        {
            throw new JsonException("Invalid sales order status value");
        }

        var catalogIdValue = root.GetProperty("catalogToBuyFrom").GetString()
            ?? throw new JsonException("catalogToBuyFrom is required");

        var receiptDate = root.GetProperty("receiptDate").GetDateTime();
        var completitionDate = root.GetProperty("completitionDate").GetDateTime();

        var buyerIdValue = root.GetProperty("buyer").GetString()
            ?? throw new JsonException("buyer is required");

        var itemsElement = root.GetProperty("items");
        if (itemsElement.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException("items must be an array");
        }

        var items = new List<SalesOrderItem>();
        foreach (var itemElement in itemsElement.EnumerateArray())
        {
            var productIdValue = itemElement.GetProperty("productId").GetString()
                ?? throw new JsonException("productId is required for each item");

            var unitPriceElement = itemElement.GetProperty("unitPrice");
            if (!unitPriceElement.TryGetProperty("amount", out var amountElement))
            {
                throw new JsonException("unitPrice.amount is required");
            }

            if (!unitPriceElement.TryGetProperty("currency", out var currencyElement))
            {
                throw new JsonException("unitPrice.currency is required");
            }

            var amount = amountElement.GetDecimal();
            var currencyCode = currencyElement.GetString()
                ?? throw new JsonException("unitPrice.currency must be a valid string");

            var quantity = itemElement.GetProperty("quantityToSell").GetInt32();
            
            var productName = itemElement.GetProperty("productName").GetString()
                ?? throw new JsonException("productName is required for each item");

            var unitPrice = new Money(amount, new Currency(currencyCode));
            var salesOrderItem = new SalesOrderItem(new ProductId(productIdValue), unitPrice, quantity, productName);

            if (itemElement.TryGetProperty("inventoryId", out var inventoryElement) &&
                inventoryElement.ValueKind == JsonValueKind.String)
            {
                var inventoryIdValue = inventoryElement.GetString();
                if (!string.IsNullOrWhiteSpace(inventoryIdValue))
                {
                    salesOrderItem.InventoryId = new InventoryId(inventoryIdValue);
                }
            }

            items.Add(salesOrderItem);
        }

        var salesOrder = new SalesOrder(
            orderCode,
            new Shared.Domain.Model.ValueObjects.PurchaseOrderId(purchaseOrderIdValue),
            items,
            status,
            new Shared.Domain.Model.ValueObjects.CatalogId(catalogIdValue),
            receiptDate,
            completitionDate,
            AccountId.Create(buyerIdValue));

        if (!string.IsNullOrWhiteSpace(idValue) && ObjectId.TryParse(idValue, out var objectId))
        {
            salesOrder.Id = objectId;
        }

        return salesOrder;
    }

    public override void Write(Utf8JsonWriter writer, SalesOrder value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("orderCode", value.OrderCode);
        writer.WriteString("purchaseOrderId", value.PurchaseOrderId.GetId);
        writer.WriteString("status", value.Status.ToString());
        writer.WriteString("catalogToBuyFrom", value.CatalogToBuyFrom.GetId());
        writer.WriteString("receiptDate", value.ReceiptDate);
        writer.WriteString("completitionDate", value.CompletitionDate);
        writer.WriteString("buyer", value.AccountId.GetId);

        writer.WriteStartArray("items");
        if (value.Items != null)
        {
            foreach (var item in value.Items)
            {
                writer.WriteStartObject();
                writer.WriteString("productId", item.ProductId?.GetId ?? throw new JsonException("Sales order item is missing productId"));

                if (item.InventoryId != null)
                {
                    writer.WriteString("inventoryId", item.InventoryId.GetId);
                }

                writer.WriteStartObject("unitPrice");
                writer.WriteNumber("amount", item.UnitPrice?.GetAmount() ?? 0);
                writer.WriteString("currency", item.UnitPrice?.GetCurrencyCode() ?? nameof(EValidCurrencyCodes.USD));
                writer.WriteEndObject();

                writer.WriteNumber("quantityToSell", item.QuantityToSell);
                writer.WriteEndObject();
            }
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}