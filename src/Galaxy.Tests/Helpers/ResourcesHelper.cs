using System.Reflection;
using System.Collections.Generic;
using System;
using System.IO;

namespace Codestellation.Galaxy.Tests.Helpers
{
    public static class ResourcesHelper
    {
        public static void ExtractEmbeddedResource(string outputDir, string resourceLocation, IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + @"." + file))
                {
                    using (System.IO.FileStream fileStream = new System.IO.FileStream(System.IO.Path.Combine(outputDir, file), System.IO.FileMode.Create))
                    {
                        stream.CopyTo(fileStream);
                        stream.Flush();
                        fileStream.Close();
                    }
                }
            }
        }

        /// <returns>output directory</returns>
        public static void ExtractEmbeddedAndRename(string outputDir, string resourceLocation, string resourceName, string outputName)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            ResourcesHelper.ExtractEmbeddedResource(outputDir, resourceLocation, new string[] { resourceName });

            File.Move(
                Path.Combine(outputDir, resourceName),
                Path.Combine(outputDir, outputName));
        }
    }
}
