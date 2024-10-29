using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;

namespace dictionary;

public class DictionaryOptions
{
    [JsonConverter(typeof(DictionaryTypeIntJsonConverter))]
    public IDictionary<Type, int> ModelSettings { get; set; } = new Dictionary<Type, int>()
    {
        { typeof(Models.Foo), 100 },
        { typeof(Models.Bar), 200 },
    };

    public IDictionary<string, int> Strings { get; set; } = new Dictionary<string, int>();
}

