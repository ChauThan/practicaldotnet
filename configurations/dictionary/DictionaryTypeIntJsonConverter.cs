using System.Text.Json;
using System.Text.Json.Serialization;

namespace dictionary;

public class DictionaryTypeIntJsonConverter : JsonConverter<IDictionary<Type, int>>
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(IDictionary<Type, int>);
    }
    
    public override IDictionary<Type, int>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
        }
        
        var dictionary = new Dictionary<Type, int>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("JsonTokenType was not PropertyName");
            }

            var propertyName = reader.GetString();

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new JsonException("Failed to get property name");
            }

            reader.Read();

            var type = Type.GetType(propertyName);
            if (type is null)
            {
                throw new JsonException("Failed to get type");
            }
            dictionary.Add(type, ExtractValue(ref reader, options));
        }

        return dictionary;
    }

    public override void Write(Utf8JsonWriter writer, IDictionary<Type, int> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var (key, val) in value)
        {
            if (string.IsNullOrWhiteSpace(key?.FullName))
            {
                continue;
            }
            
            writer.WritePropertyName(key.FullName);
            writer.WriteNumberValue(val);
        }
        
        writer.WriteEndObject();
    }

    private int ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (reader.TryGetInt32(out var result))
        {
            return result;
        }
        
        throw new JsonException("JsonTokenType was not int");
    }
}