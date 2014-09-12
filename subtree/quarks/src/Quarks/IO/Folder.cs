using System;
using System.IO;
using System.Linq;

namespace Codestellation.Quarks.IO
{
    internal static class Folder
    {
        public static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        public static string Combine(params string[] folders)
        {
            var combined = Path.Combine(folders);

            return ToFullPath(combined);
        }

        public static void EnsureExists(string folder)
        {
            var fullPath = ToFullPath(folder);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }

        public static void EnsureDeleted(string folder)
        {
            var fullPath = ToFullPath(folder);
            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
            }
        }

        public static FileInfo[] EnumerateFiles(string folder)
        {
            var fullPath = ToFullPath(folder);
            return Directory.EnumerateFiles(fullPath)
                .Select(x => new FileInfo(x))
                .ToArray();
        }

        public static bool Exists(string folder)
        {
            var fullPath = ToFullPath(folder);
            return Directory.Exists(fullPath);
        }

        public static string ToFullPath(string path)
        {
            return Path.Combine(BasePath, path);
        }

        public static void Copy(string sourcePath, string targetPath)
        {
            var source = new DirectoryInfo(ToFullPath(sourcePath));
            var target = new DirectoryInfo(ToFullPath(targetPath));

            if (!source.Exists)
            {
                return;
            }
            if (!target.Exists)
            {
                target.Create();
            }

            // Copy each file into the new directory.
            foreach (FileInfo sourceFile in source.GetFiles())
            {
                var targetFilename = Path.Combine(target.ToString(), sourceFile.Name);
                sourceFile.CopyTo(targetFilename, true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo sourceSubdirectory in source.GetDirectories())
            {
                var targetSubdirectory = Path.Combine(target.ToString(), sourceSubdirectory.Name);
                Copy(sourceSubdirectory.FullName, targetSubdirectory);
            }
        }
    }
}