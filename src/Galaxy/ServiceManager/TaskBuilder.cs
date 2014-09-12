using System;
using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager
{
    public class TaskBuilder
    {
        private readonly Options _options;

        public TaskBuilder(Options options)
        {
            _options = options;
        }

        public DeploymentTask DeployServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("DeployService", deployment)
                .Add(BackupService(deployment))
                .Add(InstallPackage(deployment, deploymentFeed, FileList.Empty))
                .Add(OverrideFiles(deployment));
        }

        public DeploymentTask UpdateServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("UpdateService", deployment)
                .Add(StopService(deployment))
                .Add(BackupService(deployment))
                .Add(ClearBinaries(deployment))
                .Add(InstallPackage(deployment, deploymentFeed, deployment.KeepOnUpdate.Clone()))
                .Add(OverrideFiles(deployment));
        }

        public DeploymentTask InstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("InstallService", deployment)
                .Add(InstallService(deployment));
        }

        public DeploymentTask UninstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("UninstallService", deployment)
                .Add(StopService(deployment))
                .Add(UninstallService(deployment))
                .Add(UninstallPackage(deployment));
        }

        public DeploymentTask StartServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("StartService", deployment)
                .Add(StartService(deployment));
        }

        public DeploymentTask StopServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("StopService", deployment)
                .Add(StopService(deployment));
        }

        private static DeploymentTask CreateDeployTask(string name, Deployment deployment)
        {
            var deployLogFolder = deployment.GetDeployLogFolder();
            Folder.EnsureExists(deployLogFolder);
            
            var filename = string.Format("{0}.{1:yyyy-MM-dd_HH.mm.ss}.log", name, DateTime.Now);
            var fullPath = Path.Combine(deployLogFolder, filename);
            var logStream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            
            return new DeploymentTask(name, deployment.Id, logStream);
        }

        private IOperation BackupService(Deployment deployment)
        {
            var serviceName = deployment.GetServiceName();
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var backupFolder = deployment.GetBackupFolder();
            return new BackupService(serviceName, serviceFolder, backupFolder);
        }

        private IOperation ClearBinaries(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var keepOnUpdate = deployment.KeepOnUpdate;
            return new ClearBinaries(serviceFolder, keepOnUpdate);
        }

        private IOperation InstallPackage(Deployment deployment, NugetFeed deploymentFeed, FileList keepOnUpdate)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var packageId = deployment.PackageId;
            var feedUri = deploymentFeed.Uri;
            var packageVersion = deployment.PackageVersion;
            var orders = new[] { new InstallPackageOrder(packageId, feedUri, packageVersion) };
            return new InstallPackage(serviceFolder, orders, keepOnUpdate);
        }

        private IOperation UninstallPackage(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            return new UninstallPackage(serviceFolder);
        }

        private IOperation StartService(Deployment deployment)
        {
            var serviceName = deployment.GetServiceName();
            return new StartService(serviceName);
        }

        private IOperation StopService(Deployment deployment)
        {
            var serviceName = deployment.GetServiceName();
            return new StopService(serviceName);
        }

        private IOperation InstallService(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var hostFileName = deployment.GetServiceHostFileName();
            var instanceName = deployment.InstanceName;
            return new InstallService(serviceFolder, hostFileName, instanceName);
        }

        private IOperation UninstallService(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var hostFileName = deployment.GetServiceHostFileName();
            var instanceName = deployment.InstanceName;
            return new UninstallService(serviceFolder, hostFileName, instanceName);
        }

        private IOperation OverrideFiles(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var serviceFilesFolder = deployment.GetFilesFolder();
            var keepOnUpdate = deployment.KeepOnUpdate.Clone();
            return new OverrideFiles(serviceFolder, serviceFilesFolder, keepOnUpdate);
        }
    }
}