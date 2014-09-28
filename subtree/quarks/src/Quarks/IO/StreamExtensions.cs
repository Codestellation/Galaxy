using System.IO;

namespace Codestellation.Quarks.IO
{
    internal static class StreamExtensions
    {
        public static void CopyTo(this Stream source, Stream destination, byte[] buffer)
        {
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, bytesRead);
            }
        }
        
        public static void ExportTo(this Stream self, string fileName, bool overwrite = false)
        {
            var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;
            using (var filestream = File.Open(fileName, fileMode, FileAccess.Write))
            {
                self.CopyTo(filestream);
            }
        }

        public static string ExportToTempFile(this Stream self)
        {
            var fileName = Path.GetTempFileName();
            self.ExportTo(fileName, true);
            return fileName;
        }
    }
}
