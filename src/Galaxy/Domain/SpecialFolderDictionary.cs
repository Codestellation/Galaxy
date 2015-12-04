using System.Collections.Generic;
using System.Linq;

namespace Codestellation.Galaxy.Domain
{
    public class SpecialFolderDictionary : Dictionary<string, SpecialFolder>
    {
        public static readonly string DeployFolder = "DeployFolder";
        public static readonly string DeployLogsFolder = "DeployLogsFolder";
        public static readonly string BackupFolder = "BackupFolder";
        public static readonly string FileOverrides = "FileOverrides";

        public static readonly string Logs = "Logs";
        public static readonly string Configs = "Configs";
        public static readonly string Data = "Data";

        private static readonly HashSet<string> HostFolderKeys = new HashSet<string> { Logs, Configs, Data };

        public void Add(SpecialFolder folder)
        {
            Add(folder.Purpose, folder);
        }

        public bool Remove(SpecialFolder folder)
        {
            return Remove(folder.Purpose);
        }

        public IEnumerable<SpecialFolder> HostFolders => Values.Where(x => HostFolderKeys.Contains(x.Purpose));
    }
}