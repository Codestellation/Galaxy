using System.Configuration;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;

namespace Codestellation.Galaxy.ServiceManager
{
    public class Build
    {
        static string _targetPath;

        static Build()
        {
            _targetPath = ConfigurationManager.AppSettings["appsdestination"];
        }

        public static DeploymentTask DeployServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("DeployService", deployment, deploymentFeed, _targetPath)
                .InstallPackage()
                .InstallPackage(HostDeployHelper.CreateDeployment(deployment), HostDeployHelper.CreateFeed())
                .CopyNugetsToRoot()
                .ProvideServiceConfig()
                .ConfigurePlatform()
                .DeployUserConfig();
        }

        public static DeploymentTask InstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("InstallService", deployment, deploymentFeed, _targetPath)
                .ProvideServiceConfig()
                .InstallService()
                .DeployUserConfig();
        }

        public static DeploymentTask UninstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("UninstallService", deployment, deploymentFeed, _targetPath)
                .StopService()
                .UninstallService()
                .UninstallPackage();
        }
        public static DeploymentTask StartServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("StartService", deployment, deploymentFeed, _targetPath)
                .StartService();
        }
        public static DeploymentTask StopServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("StopService", deployment, deploymentFeed, _targetPath)
                .StopService();
        }
    }
}
