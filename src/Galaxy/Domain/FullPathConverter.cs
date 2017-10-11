using System;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Domain
{
    public class FullPathConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteValue((string)(FullPath)value);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => (FullPath)(string)reader.Value;

        public override bool CanConvert(Type objectType) => objectType == typeof(FullPath);
    }
}