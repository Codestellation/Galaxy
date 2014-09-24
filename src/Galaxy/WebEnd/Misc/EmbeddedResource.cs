using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Nancy;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    public struct EmbeddedResource
    {
        public readonly string Etag;
        public readonly Assembly Assembly;
        public readonly string ResourcePath;
        public readonly string ContentType;

        public EmbeddedResource(Assembly assembly, string resourcePath) : this()
        {
            Assembly = assembly;
            ResourcePath = resourcePath;
            Etag = GenerateETag();
            ContentType = MimeTypes.GetMimeType(resourcePath);
        }

        public Stream GetContent()
        {
            return Assembly.GetManifestResourceStream(ResourcePath);
        }

        private string GenerateETag()
        {
            using (var stream = GetContent())
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(stream);
                return string.Concat("\"", ByteArrayToString(hash), "\"");
            }
        }

        private static string ByteArrayToString(byte[] data)
        {
            var output = new StringBuilder(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                output.Append(data[i].ToString("X2"));
            }

            return output.ToString();
        }
    }
}