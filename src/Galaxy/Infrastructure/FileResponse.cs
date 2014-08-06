using System.IO;
using Nancy;

namespace Codestellation.Galaxy.Infrastructure
{
    public class FileResponse : Response
    {
        public FileResponse(string fullPath)
        {
            ContentType = MimeTypes.GetMimeType(fullPath);
            var content = File.OpenRead(fullPath);

            Contents = content.CopyTo;
        }
    }
}