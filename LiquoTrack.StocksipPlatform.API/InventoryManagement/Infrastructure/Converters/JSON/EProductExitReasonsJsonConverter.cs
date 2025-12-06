using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using Microsoft.OpenApi.Extensions;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Infrastructure.Converters.JSON;

/// <summary>
///     Json converter for EProductExitReasons enum.
/// </summary>
public class EProductExitReasonsJsonConverter : JsonConverter<EProductExitReasons>
{
    public override EProductExitReasons Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Enum.Parse<EProductExitReasons>(value ?? throw new InvalidOperationException());
    }

    public override void Write(Utf8JsonWriter writer, EProductExitReasons value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.GetDisplayName());
    }
}