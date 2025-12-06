using System.Text.Json;
using System.Text.Json.Serialization;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Converters.JSON;

public class EOrderStatusJsonConverter : JsonConverter<EOrderStatus>
{
    public override EOrderStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var enumString = reader.GetString();
        return Enum.Parse<EOrderStatus>(enumString!, true);
    }

    public override void Write(Utf8JsonWriter writer, EOrderStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}