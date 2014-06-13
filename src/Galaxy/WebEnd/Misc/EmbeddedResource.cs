using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Codestellation.Galaxy.WebEnd.Misc
{
    internal struct EmbeddedResource
    {
        public readonly string Etag;
        public readonly string ResourcePath;

        public EmbeddedResource(string resourcePath) : this()
        {
            ResourcePath = resourcePath;
            Etag = GenerateETag();
        }

        public Stream GetContent()
        {
            return EmbeddedFileContentResponse.Assembly.GetManifestResourceStream(ResourcePath);
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