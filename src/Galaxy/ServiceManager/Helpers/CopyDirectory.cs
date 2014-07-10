using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    internal static class CopyDirectory
    {
        public static void CopyIncludeSubfoldersTo(this string source, string destination)
        {
            Copy(source, destination, true);
        }

        private  static void Copy(string source, string destination, bool includeSubfolders)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(source);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: "+ source);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destination, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (includeSubfolders)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destination, subdir.Name);
                    Copy(subdir.FullName, temppath, includeSubfolders);
                }
            }
        }
    }
}
