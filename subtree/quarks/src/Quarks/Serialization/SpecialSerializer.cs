using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Codestellation.Quarks.Serialization
{
    internal class SpecialSerializer
    {
        public static string ToBase64(object random)
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, random);
            var buffer = stream.GetBuffer();
            return Convert.ToBase64String(buffer, 0, (int) (stream.Position));
        }

        public static T FromBase64<T>(string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            var formatter = new BinaryFormatter();
            var newStrem = new MemoryStream(bytes);

            return (T) formatter.Deserialize(newStrem);
        }
    }
}
