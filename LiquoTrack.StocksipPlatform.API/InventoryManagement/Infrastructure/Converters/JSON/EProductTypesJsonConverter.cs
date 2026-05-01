using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using Microsoft.OpenApi.Extensions;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Converters.JSON;

/// <summary>
///     Converter for EProductTypes enum to JSON string representation.
/// </summary>
public class EProductTypesJsonConverter : JsonConverter<EProductTypes>
{
    public override EProductTypes Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Enum.Parse<EProductTypes>(value ?? throw new InvalidOperationException());
    }

    public override void Write(Utf8JsonWriter writer, EProductTypes value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.GetDisplayName());
    }
}