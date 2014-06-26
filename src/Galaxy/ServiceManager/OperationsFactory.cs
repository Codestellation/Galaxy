using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public class OperationsFactory : IOperationsFactory
    {

        public ServiceOperation GetInstallPackageOp(string targetPath, Deployment deployment, NugetFeed feed)
        {
            return new InstallPackage(targetPath, deployment, feed);
        }

        public ServiceOperation GetCopyNugetsToRootOp(string targetPath, Deployment deployment, NugetFeed feed)
        {
            return new CopyNugetsToRoot(targetPath, deployment, feed);
        }

        public ServiceOperation GetProvideServiceConfigOp(string targetPath, Deployment deployment, NugetFeed feed)
        {
            return new ProvideServiceConfig(targetPath, deployment, feed);
        }

        public ServiceOperation GetInstallServiceOp(string targetPath, Deployment deployment, NugetFeed feed)
        {
            return new InstallService(targetPath, deployment, feed);
        }

        public ServiceOperation GetStartServiceOp(string targetPath, Deployment deployment, NugetFeed feed)
        {
            return new StartService(targetPath, deployment, feed);
        }

        public ServiceOperation GetStopServiceOp(string targetPath, Deployment deployment, NugetFeed feed)
        {
            return new StopService(targetPath, deployment, feed);
        }

        public ServiceOperation GetUninstallServiceOp(string targetPath, Deployment deployment, NugetFeed feed)
        {
            return new UninstallService(targetPath, deployment, feed);
        }

        public ServiceOperation GetUninstallPackageOp(string targetPath, Deployment deployment, NugetFeed feed)
        {
            return new UninstallPackage(targetPath, deployment, feed);
        }
    }
}
