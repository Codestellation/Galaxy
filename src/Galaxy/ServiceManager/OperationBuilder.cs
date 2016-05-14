using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public class OperationBuilder
    {
        public IOperation BackupService(Deployment deployment)
        {
            var serviceName = deployment.GetServiceName();
            var serviceFolder = deployment.GetDeployFolder();
            var backupFolder = deployment.GetBackupFolder();
            return new BackupService(serviceName, serviceFolder, backupFolder);
        }

        public IOperation ClearBinaries(Deployment deployment, FileList keepOnUpdate = null)
        {
            var serviceFolder = deployment.GetDeployFolder();
            keepOnUpdate = keepOnUpdate ?? deployment.KeepOnUpdate.Clone();
            return new ClearBinaries(serviceFolder, keepOnUpdate);
        }

        public IOperation InstallPackage(Deployment deployment, NugetFeed deploymentFeed, FileList keepOnUpdate)
        {
            var serviceFolder = deployment.GetDeployFolder();
            var packageId = deployment.PackageId;
            var feedUri = deploymentFeed.Uri;
            var packageVersion = deployment.PackageVersion;
            var packageDetails = new PackageDetails(packageId, feedUri, packageVersion);
            return new InstallPackage(serviceFolder, packageDetails, keepOnUpdate);
        }

        public IOperation UninstallPackage(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder();
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
            return new StopService(serviceName) { SkipIfNotFound = skipIfNotFound };
        }

        public IOperation InstallService(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder();
            var hostFileName = deployment.GetServiceHostFileName();
            var instanceName = deployment.InstanceName;
            return new InstallService(serviceFolder, hostFileName, instanceName);
        }

        public IOperation UninstallService(Deployment deployment, bool skipIfNotFound = false)
        {
            var serviceFolder = deployment.GetDeployFolder();
            var hostFileName = deployment.GetServiceHostFileName();
            var instanceName = deployment.InstanceName;
            return new UninstallService(serviceFolder, hostFileName, instanceName)
            {
                SkipIfNotFound = skipIfNotFound
            };
        }

        public IOperation RestoreFrom(Deployment deployment, string backupFolder)
        {
            var serviceFolder = deployment.GetDeployFolder();
            return new RestoreFromBackup(serviceFolder, backupFolder);
        }

        public IOperation DeleteFolders(Deployment deployment)
        {
            var folders = deployment.ServiceFolders.Values.Select(x => x.FullPath).ToArray();
            return new DeleteFolders(folders);
        }

        public IOperation PublishDeletedEvent(Deployment deployment)
        {
            return new PublishDeploymentDeletedEvent(deployment);
        }

        public IOperation DeployConsulConfig(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder();
            return new DeployHostConfig(serviceFolder);
        }

        public IOperation EnsureFolders()
        {
            return new EnsureFolders();
        }
    }
}