using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace Codestellation.Galaxy.Tests.Helpers
{
    public static class EmbeddedResource
    {
        public static void Extract(string outputDir, string resourceLocation, IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                Extract(outputDir, resourceLocation, file);
            }
        }

        public static void Extract(string outputDir, string resourceLocation, string resourceName)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + @"." + resourceName))
            {
                using (FileStream fileStream = new FileStream(Path.Combine(outputDir, resourceName), FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                    stream.Flush();
                    fileStream.Close();
                }
            }
        }

        public static void ExtractAndRename(string outputDir, string resourceLocation, string resourceName, string outputName)
        {
            Extract(outputDir, resourceLocation, resourceName);

            File.Move(
                Path.Combine(outputDir, resourceName),
                Path.Combine(outputDir, outputName));
        }
    }
}
