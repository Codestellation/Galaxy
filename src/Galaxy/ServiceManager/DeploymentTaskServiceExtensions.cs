using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public static class DeploymentTaskServiceExtensions
    {
        public static DeploymentTask InstallPackage(this DeploymentTask deploymentTask, string deployPath, InstallPackage.InstallPackageOrder[] orders)
        {
            deploymentTask.Add
                (
                    new InstallPackage(deployPath, orders)
                );
            return deploymentTask;
        }

        public static DeploymentTask InstallService(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new InstallService(deploymentTask.BasePath, deploymentTask.Deployment)
                );
            return deploymentTask;
        }

        public static DeploymentTask ProvideServiceConfig(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new ProvideHostConfig(deploymentTask.BasePath, deploymentTask.Deployment)
                );
            return deploymentTask;
        }

        public static DeploymentTask StartService(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new StartService(deploymentTask.BasePath, deploymentTask.Deployment)
                );
            return deploymentTask;
        }

        public static DeploymentTask StopService(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new StopService(deploymentTask.BasePath, deploymentTask.Deployment)
                );
            return deploymentTask;
        }

        public static DeploymentTask UninstallPackage(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new UninstallPackage(deploymentTask.BasePath, deploymentTask.Deployment)
                );
            return deploymentTask;
        }

        public static DeploymentTask UninstallService(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new UninstallService(deploymentTask.BasePath, deploymentTask.Deployment)
                );
            return deploymentTask;
        }
        public static DeploymentTask CopyNugetsToRoot(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new CopyNugetsToRoot(deploymentTask.BasePath, deploymentTask.Deployment)
                );
            return deploymentTask;
        }
        public static DeploymentTask DeployUserConfig(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new DeployUserConfig(deploymentTask.BasePath, deploymentTask.Deployment)
                );
            return deploymentTask;
        }

        public static DeploymentTask ConfigurePlatform(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new ConfigurePlatform(deploymentTask.BasePath, deploymentTask.Deployment)
                );
            return deploymentTask;
        }
    }
}
