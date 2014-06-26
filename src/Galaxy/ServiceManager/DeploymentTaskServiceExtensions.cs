using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public static class DeploymentTaskServiceExtensions
    {
        public static DeploymentTask InstallPackageOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Operations.AddLast
                (
                    new InstallPackage(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask InstallPackageOp(this DeploymentTask deploymentTask, NugetFeed feed)
        {
            deploymentTask.Operations.AddLast
                (
                    new InstallPackage(deploymentTask.TargetPath, deploymentTask.Deployment, feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask InstallServiceOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Operations.AddLast
                (
                    new InstallService(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask ProvideServiceConfigOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Operations.AddLast
                (
                    new ProvideServiceConfig(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask StartServiceOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Operations.AddLast
                (
                    new StartService(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask StopServiceOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Operations.AddLast
                (
                    new StopService(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask UninstallPackageOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Operations.AddLast
                (
                    new UninstallPackage(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask UninstallServiceOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Operations.AddLast
                (
                    new UninstallService(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }
        public static DeploymentTask CopyNugetsToRootOp(this DeploymentTask deploymentTask)
        {
            deploymentTask.Operations.AddLast
                (
                    new CopyNugetsToRoot(deploymentTask.TargetPath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }
    }
}
