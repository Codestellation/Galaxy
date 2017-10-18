using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class RestoreFromBackup : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            var backupFolder = context.Folders.BackupFolder;
            dynamic restoreFrom = Folder.Combine((string)backupFolder, (string)context.Parameters.RestoreFrom);
            FullPath deployFolder = context.Folders.DeployFolder;
            context.BuildLog.WriteLine("Restore from {0} to {1}", restoreFrom, deployFolder);

            Folder.Copy(restoreFrom, (string)deployFolder);
        }
    }
}