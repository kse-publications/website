using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Publications.API.Serialization;

public class ResponseJsonConverter<T> : JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        PropertyInfo[] properties = typeof(T).GetProperties();

        writer.WriteStartObject();

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<IgnoreInResponseAttribute>() is not null)
            {
                continue;
            }
            
            writer.WritePropertyName(property.Name.ToLower());
            JsonSerializer.Serialize(writer, property.GetValue(value), property.PropertyType, options);
        }

        writer.WriteEndObject();
    }
}