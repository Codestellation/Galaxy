using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public class OperationBuilder
    {
        public IOperation BackupService()
        {
            return new BackupService();
        }

        public IOperation ClearBinaries()
        {
            return new ClearBinaries();
        }

        public IOperation InstallPackage()
        {
            return new InstallPackage();
        }

        public IOperation UninstallPackage()
        {
            return new UninstallPackage();
        }

        public IOperation StartService()
        {
            return new StartService();
        }

        public IOperation StopService()
        {
            return new StopService();
        }

        public IOperation InstallService()
        {
            return new InstallService();
        }

        public IOperation UninstallService()
        {
            return new UninstallService();
        }

        public IOperation RestoreFrom()
        {
            return new RestoreFromBackup();
        }

        public IOperation DeleteFolders()
        {
            return new DeleteFolders();
        }

        public IOperation PublishDeletedEvent()
        {
            return new PublishDeploymentDeletedEvent();
        }

        public IOperation DeployHostConfig()
        {
            return new DeployHostConfig();
        }

        public IOperation EnsureFolders()
        {
            return new EnsureFolders();
        }

        public IOperation GetConfigSample()
        {
            return new GetConfigSample();
        }

        public IOperation DeployServiceConfig()
        {
            return new DeployServiceConfig();
        }
    }
}