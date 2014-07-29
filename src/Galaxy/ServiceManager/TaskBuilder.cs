using System;
using Codestellation.Galaxy.Domain;
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
            return new DeploymentTask("DeployService", deployment.Id)
                .Add(InstallPackage(deployment, deploymentFeed, FileList.Empty))
                .Add(ProvideHostConfig(deployment))
                .Add(ConfigurePlatform(deployment))
                .Add(DeployConfig(deployment));
        }

        private IOperation InstallPackage(Deployment deployment, NugetFeed deploymentFeed, FileList keepOnUpdate)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.RootDeployFolder);

            var orders = new[]
            {
                new InstallPackageOrder(deployment.PackageId, deploymentFeed.Uri, deployment.PackageVersion), 
                new InstallPackageOrder(_options.GetHostPackageId(), _options.HostPackageFeedUri), 

            };
            return new InstallPackage(serviceFolder, orders, keepOnUpdate);
        }

        public DeploymentTask UpdateServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.RootDeployFolder);

            var clearBinaries = new ClearBinaries(serviceFolder, deployment.KeepOnUpdate);
            return new DeploymentTask("UpdateService", deployment.Id)
                .Add(StopService(deployment))
                .Add(clearBinaries)
                .Add(InstallPackage(deployment, deploymentFeed, deployment.KeepOnUpdate.Clone()))
                .Add(ProvideHostConfig(deployment))
                .Add(ConfigurePlatform(deployment));
        }

        public DeploymentTask InstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("InstallService", deployment.Id)
                .Add(ProvideHostConfig(deployment))
                .Add(InstallService(deployment))
                .Add(DeployConfig(deployment));
        }

        public DeploymentTask UninstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.RootDeployFolder);
            return new DeploymentTask("UninstallService", deployment.Id)
                .Add(StopService(deployment))
                .Add(UninstallService(deployment))
                .Add(new UninstallPackage(serviceFolder));
        }

        public DeploymentTask StartServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("StartService", deployment.Id)
                .Add(StartService(deployment));
        }

        public DeploymentTask StopServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("StopService", deployment.Id)
                .Add(StopService(deployment));
        }

        private IOperation ConfigurePlatform(Deployment deployment)
        {
            var serviceFolder = deployment.GetDeployFolder(_options.RootDeployFolder);
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
            var serviceFolder = deployment.GetDeployFolder(_options.RootDeployFolder);
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
