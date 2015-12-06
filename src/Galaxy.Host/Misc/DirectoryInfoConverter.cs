using System;
using System.IO;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Host.Misc
{
    public class DirectoryInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DirectoryInfo);
        }

        public override bool CanRead { get; } = true;
        public override bool CanWrite { get; } = true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(((DirectoryInfo)value).FullName);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var fullPath = (string)reader.Value;

            return fullPath == null ? null : new DirectoryInfo(fullPath);
        }
    }
}