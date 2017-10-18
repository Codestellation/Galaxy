using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.DateAndTime;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class BackupService : IOperation
    {
        private TextWriter _buildLog;

        public void Execute(DeploymentTaskContext context)
        {
            _buildLog = context.BuildLog;
            ;
            var backupFolder = context.Folders.BackupFolder;

            Folder.EnsureExists((string)backupFolder);

            if (IsNothingToBackup(context.Folders.DeployFolder))
            {
                _buildLog.WriteLine("Nothing to backup");
                return;
            }

            Backup(context.Folders.DeployFolder, context.Folders.BackupFolder);
        }

        private bool IsNothingToBackup(FullPath deployFolder)
        {
            return !Folder.Exists((string)deployFolder);
        }

        private void Backup(FullPath deployFolder, FullPath backupFolder)
        {
            var timestamp = Clock.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
            var currentBackupFolder = Path.Combine((string)backupFolder, timestamp);

            Folder.Copy((string)deployFolder, currentBackupFolder);

            _buildLog.WriteLine("Successfully backed up to {0}", currentBackupFolder);
        }
    }
}