using System.IO;
using Nancy;

namespace Codestellation.Galaxy.Infrastructure
{
    public class FileResponse : Response
    {
        private string _fullPath;

        public FileResponse(string fullPath)
        {
            _fullPath = fullPath;

            ContentType = MimeTypes.GetMimeType(fullPath);
            Contents = CopyFromFile;
        }

        private void CopyFromFile(Stream responseStream)
        {
            using (var content = File.OpenRead(_fullPath))
            {
                content.CopyTo(responseStream);
            }
        }
    }
}