using System.Configuration;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentTaskBuilder
    {
        static string _targetPath;

        static DeploymentTaskBuilder()
        {
            _targetPath = ConfigurationManager.AppSettings["appsdestination"];
        }

        public static DeploymentTask DeployServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("DeployService", deployment, deploymentFeed, _targetPath)
                .InstallPackageOp()
                .InstallPackageOp(HostFeedHelper.Create())
                .CopyNugetsToRootOp()
                .ProvideServiceConfigOp();
        }

        public static DeploymentTask InstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("InstallService", deployment, deploymentFeed, _targetPath)
                .InstallPackageOp()
                .InstallPackageOp(HostFeedHelper.Create())
                .CopyNugetsToRootOp()
                .ProvideServiceConfigOp()
                .InstallServiceOp();
        }

        public static DeploymentTask UninstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("UninstallService", deployment, deploymentFeed, _targetPath)
                .StopServiceOp()
                .UninstallServiceOp()
                .UninstallPackageOp();
        }
        public static DeploymentTask StartServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("StartService", deployment, deploymentFeed, _targetPath)
                .StartServiceOp();
        }
        public static DeploymentTask StopServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return new DeploymentTask("StopService", deployment, deploymentFeed, _targetPath)
                .StopServiceOp();
        }
    }
}
