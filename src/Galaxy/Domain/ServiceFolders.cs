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

        public FullPath this[string name]
        {
            get
            {
                switch (name)
                {
                    case nameof(DeployFolder):
                        return DeployFolder;
                    case nameof(DeployLogsFolder):
                        return DeployLogsFolder;
                    case nameof(BackupFolder):
                        return BackupFolder;
                    case nameof(Logs):
                        return Logs;
                    case nameof(Configs):
                        return Configs;
                    case nameof(Data):
                        return Data;
                    default:
                        return default(FullPath);
                }
            }
            set
            {
                switch (name)
                {
                    case nameof(DeployFolder):
                        DeployFolder = value;
                        return;
                    case nameof(DeployLogsFolder):
                        DeployLogsFolder = value;
                        return;
                    case nameof(BackupFolder):
                        BackupFolder = value;
                        return ;
                    case nameof(Logs):
                        Logs = value;
                        return ;
                    case nameof(Configs):
                        Configs = value;
                        return ;
                    case nameof(Data):
                        Data = value;
                        return ;
                }
            }
        }

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

        public ServiceFolders Clone()
        {
            var result = new ServiceFolders();
            foreach (var kvp in this.ToDictionary())
            {
                result[kvp.Key] = (FullPath) kvp.Value;
            }
            return result;
        }
    }
}