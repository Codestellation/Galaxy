using System;
using System.IO;

namespace Codestellation.Galaxy.Domain
{
    public class FolderOptions
    {
        public FolderOptions()
        {
            var root = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Root.Name;

            Logs = Path.Combine(root, "logs");
            Configs = Path.Combine(root, "configs");
            Data = Path.Combine(root, "data");
        }

        public string Logs { get; set; }

        public string Configs { get; set; }

        public string Data { get; set; }
    }
}