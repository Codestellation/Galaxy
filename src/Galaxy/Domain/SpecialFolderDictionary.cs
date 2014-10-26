using System.Collections.Generic;

namespace Codestellation.Galaxy.Domain
{
    public class SpecialFolderDictionary : Dictionary<string, SpecialFolder>
    {
        public static readonly string DeployFolder = "DeployFolder";
        public static readonly string DeployLogsFolder = "DeployLogsFolder";
        public static readonly string BackupFolder = "BackupFolder";
        public static readonly string FileOverrides = "FileOverrides";

        public void Add(SpecialFolder folder)
        {
            Add(folder.Purpose, folder);
        }

        public bool Remove(SpecialFolder folder)
        {
            return Remove(folder.Purpose);
        }

    }
}