using System.Text.Json;
using System.Text.Json.Serialization;
using JsonException = System.Text.Json.JsonException;

namespace Publications.Domain.Shared.Serialization;

public class UnixTimestampJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number && 
            reader.TokenType != JsonTokenType.String)
            throw new JsonException();

        long unixTimeMilliseconds;
        if (reader.TokenType == JsonTokenType.String)
        {
            if (!long.TryParse(reader.GetString(), out unixTimeMilliseconds))
                throw new JsonException();
        } else {
            unixTimeMilliseconds = reader.GetInt64();
        }
        
        if (unixTimeMilliseconds == 0)
            return DateTime.MinValue;
        
        return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        if (value == DateTime.MinValue)
        {
            writer.WriteNumberValue(0);
            return;
        }
        
        long unixTimeMilliseconds = ((DateTimeOffset)value).ToUnixTimeMilliseconds();
        writer.WriteNumberValue(unixTimeMilliseconds);
    }
}