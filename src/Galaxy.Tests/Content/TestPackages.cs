using System.IO;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.Tests.Content
{
    public static class TestPackages
    {
        public static void CopyHostPackageTo(string folder)
        {
            Copy("Codestellation.Galaxy.Host.1.0.0.nupkg", folder);
        }

        public static void CopyTest10To(string folder)
        {
            Copy("TestNugetPackage.1.0.0.nupkg", folder);
        }

        public static void CopyTest11To(string folder)
        {
            Copy("TestNugetPackage.1.1.0.nupkg", folder);
        }

        private static void Copy(string filename, string folder)
        {
            var source = Folder.Combine("Content", filename);
            var destination = Folder.Combine(folder, filename);
            File.Copy(source, destination);
        }
    }
}