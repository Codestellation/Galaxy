using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public class OperationBuilder
    {
        private readonly Options _options;

        public OperationBuilder(Options options)
        {
            _options = options;
        }
        
        public IOperation BackupService(Deployment deployment)
        {
            var serviceName = deployment.GetServiceName();
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var backupFolder = deployment.GetBackupFolder();
            return new BackupService(serviceName, serviceFolder, backupFolder);
        }

        public IOperation ClearBinaries(Deployment deployment, FileList keepOnUpdate = null)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            keepOnUpdate = keepOnUpdate ??  deployment.KeepOnUpdate.Clone();
            return new ClearBinaries(serviceFolder, keepOnUpdate);
        }

        public IOperation InstallPackage(Deployment deployment, NugetFeed deploymentFeed, FileList keepOnUpdate)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var packageId = deployment.PackageId;
            var feedUri = deploymentFeed.Uri;
            var packageVersion = deployment.PackageVersion;
            var orders = new[] { new InstallPackageOrder(packageId, feedUri, packageVersion) };
            return new InstallPackage(serviceFolder, orders, keepOnUpdate);
        }

        public IOperation UninstallPackage(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            return new UninstallPackage(serviceFolder);
        }

        public IOperation StartService(Deployment deployment)
        {
            var serviceName = deployment.GetServiceName();
            return new StartService(serviceName);
        }

        public IOperation StopService(Deployment deployment, bool skipIfNotFound)
        {
            var serviceName = deployment.GetServiceName();
            return new StopService(serviceName){SkipIfNotFound = skipIfNotFound};
        }

        public IOperation InstallService(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var hostFileName = deployment.GetServiceHostFileName();
            var instanceName = deployment.InstanceName;
            return new InstallService(serviceFolder, hostFileName, instanceName);
        }

        public IOperation UninstallService(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var hostFileName = deployment.GetServiceHostFileName();
            var instanceName = deployment.InstanceName;
            return new UninstallService(serviceFolder, hostFileName, instanceName);
        }

        public IOperation OverrideFiles(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var serviceFilesFolder = deployment.GetFilesFolder();
            var keepOnUpdate = deployment.KeepOnUpdate.Clone();
            return new OverrideFiles(serviceFolder, serviceFilesFolder, keepOnUpdate);
        }

        public IOperation RestoreFrom(Deployment deployment, string backupFolder)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            return new RestoreFromBackup(serviceFolder, backupFolder);
        }
    }
}