using Codestellation.Galaxy.Domain;
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
            var serviceFolder = deployment.GetDeployFolder(_options.RootDeployFolder);

            var orders = new[]
            {
                new InstallPackageOrder(deployment.PackageId, deploymentFeed.Uri, deployment.PackageVersion), 
                new InstallPackageOrder(_options.GetHostPackageId(), _options.HostPackageFeedUri), 

            };

            return new DeploymentTask("DeployService", deployment, _options.RootDeployFolder)
                .InstallPackage(serviceFolder, orders)
                .ProvideServiceConfig()
                .ConfigurePlatform()
                .DeployUserConfig();
        }

        public DeploymentTask InstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("InstallService", deployment, _options.GetDeployFolder())
                .ProvideServiceConfig()
                .InstallService()
                .DeployUserConfig();
        }

        public DeploymentTask UninstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("UninstallService", deployment, _options.GetDeployFolder())
                .StopService()
                .UninstallService()
                .UninstallPackage();
        }
        public DeploymentTask StartServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("StartService", deployment, _options.GetDeployFolder())
                .StartService();
        }
        public DeploymentTask StopServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("StopService", deployment, _options.GetDeployFolder())
                .StopService();
        }
    }
}
