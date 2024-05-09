using System.Text.Json;
using System.Text.Json.Serialization;
using JsonException = System.Text.Json.JsonException;

namespace Publications.Domain.Shared;

public class UnixTimestampJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException();
        }

        long unixTimeMilliseconds = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        long unixTimeMilliseconds = ((DateTimeOffset)value).ToUnixTimeMilliseconds();
        writer.WriteNumberValue(unixTimeMilliseconds);
    }
}