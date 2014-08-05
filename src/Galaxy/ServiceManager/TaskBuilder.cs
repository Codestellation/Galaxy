using System;
using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager.Helpers;
using Codestellation.Galaxy.ServiceManager.Operations;

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
                .Add(InstallPackage(deployment, deploymentFeed, FileList.Empty))
                .Add(ProvideHostConfig(deployment))
                .Add(ConfigurePlatform(deployment))
                .Add(DeployConfig(deployment));
        }

        private IOperation InstallPackage(Deployment deployment, NugetFeed deploymentFeed, FileList keepOnUpdate)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());

            var orders = new[]
            {
                new InstallPackageOrder(deployment.PackageId, deploymentFeed.Uri, deployment.PackageVersion), 
                new InstallPackageOrder(_options.GetHostPackageId(), _options.HostPackageFeedUri), 

            };
            return new InstallPackage(serviceFolder, orders, keepOnUpdate);
        }

        public DeploymentTask UpdateServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());

            var clearBinaries = new ClearBinaries(serviceFolder, deployment.KeepOnUpdate);
            return CreateDeployTask("UpdateService", deployment)
                .Add(StopService(deployment))
                .Add(clearBinaries)
                .Add(InstallPackage(deployment, deploymentFeed, deployment.KeepOnUpdate.Clone()))
                .Add(ProvideHostConfig(deployment))
                .Add(ConfigurePlatform(deployment));
        }

        public DeploymentTask InstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("InstallService", deployment)
                .Add(ProvideHostConfig(deployment))
                .Add(InstallService(deployment))
                .Add(DeployConfig(deployment));
        }

        public DeploymentTask UninstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            return CreateDeployTask("UninstallService", deployment)
                .Add(StopService(deployment))
                .Add(UninstallService(deployment))
                .Add(new UninstallPackage(serviceFolder));
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

            Folder.Ensure(deployLogFolder);
            var filename = string.Format("{0}.{1:yyyy-MM-dd_HH.mm.ss}.log", name, DateTime.Now);
            var fullPath = Path.Combine(deployLogFolder, filename);

            var logStream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            
            return new DeploymentTask(name, deployment.Id, logStream);
        }

        private IOperation ConfigurePlatform(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            return new ConfigurePlatform(serviceFolder, _options.GetHostFileName(), deployment.AssemblyQualifiedType);
        }

        private IOperation StartService(Deployment deployment)
        {
            return new StartService(deployment.GetServiceName());
        }

        private IOperation StopService(Deployment deployment)
        {
            return new StopService(deployment.GetServiceName());
        }

        private IOperation InstallService(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            return new InstallService(serviceFolder, _options.GetHostFileName());
        }

        private IOperation UninstallService(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            return new UninstallService(serviceFolder, _options.GetHostFileName());
        }

        private IOperation DeployConfig(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var hostFileName = _options.GetHostFileName();
            var content = deployment.ConfigFileContent;

            return new DeployUserConfig(serviceFolder, hostFileName, content);
        }

        private IOperation ProvideHostConfig(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.GetDeployFolder());
            var settings = new ServiceConfig(deployment);
            return new ProvideHostConfig(serviceFolder, settings);
        }
    }
}
