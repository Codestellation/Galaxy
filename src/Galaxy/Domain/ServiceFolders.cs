using System.Collections.Generic;

namespace Codestellation.Galaxy.Domain
{
    public class ServiceFolders
    {
        public FullPath DeployFolder { get; set; }
        public FullPath DeployLogsFolder { get; set; }
        public FullPath BackupFolder { get; set; }

        public FullPath Logs { get; set; }
        public FullPath Configs { get; set; }
        public FullPath Data { get; set; }

        public string[] ToArray()
        {
            return new[]
            {
                (string)DeployFolder,
                (string)DeployLogsFolder,
                (string)BackupFolder,
                (string)Logs,
                (string)Configs,
                (string)Data
            };
        }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { nameof(DeployFolder), DeployFolder.ToString() },
                { nameof(DeployLogsFolder), DeployLogsFolder.ToString() },
                { nameof(BackupFolder), BackupFolder.ToString() },
                { nameof(Logs), Logs.ToString() },
                { nameof(Configs), Configs.ToString() },
                { nameof(Data), Data.ToString() }
            };
        }
    }
}