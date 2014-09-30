using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class RestoreFromBackup : IOperation
    {
        private readonly string _serviceFolder;
        private readonly string _backupFolder;

        public RestoreFromBackup(string serviceFolder, string backupFolder)
        {
            _serviceFolder = serviceFolder;
            _backupFolder = backupFolder;
         
        }

        public void Execute(DeploymentTaskContext context)
        {
            context.BuildLog.WriteLine("Restore from {0} to {1}", _backupFolder, _serviceFolder);

            Folder.Copy(_backupFolder, _serviceFolder);
        }
    }
}