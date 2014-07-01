using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public static class DeploymentTaskServiceExtensions
    {
        public static DeploymentTask InstallPackageOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new InstallPackage(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask InstallPackageOp(this DeploymentTask deploymentTask, Deployment deployment, NugetFeed feed)
        {
            deploymentTask.Add
                (
                    new InstallPackage(deploymentTask.TargetPath, deployment, feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask InstallServiceOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new InstallService(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask ProvideServiceConfigOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new ProvideServiceConfig(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask StartServiceOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new StartService(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask StopServiceOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new StopService(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask UninstallPackageOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new UninstallPackage(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask UninstallServiceOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new UninstallService(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }
        public static DeploymentTask CopyNugetsToRootOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new CopyNugetsToRoot(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }
    }
}
