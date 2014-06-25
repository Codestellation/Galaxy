using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public class OperationsFactory : IOperationsFactory
    {

        public ServiceOperation GetInstallPackageOp(string targetPath, ServiceApp serviceApp, NugetFeed feed)
        {
            return new InstallPackage(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetCopyNugetsToRootOp(string targetPath, ServiceApp serviceApp, NugetFeed feed)
        {
            return new CopyNugetsToRoot(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetProvideServiceConfigOp(string targetPath, ServiceApp serviceApp, NugetFeed feed)
        {
            return new ProvideServiceConfig(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetInstallServiceOp(string targetPath, ServiceApp serviceApp, NugetFeed feed)
        {
            return new InstallService(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetStartServiceOp(string targetPath, ServiceApp serviceApp, NugetFeed feed)
        {
            return new StartService(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetStopServiceOp(string targetPath, ServiceApp serviceApp, NugetFeed feed)
        {
            return new StopService(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetUninstallServiceOp(string targetPath, ServiceApp serviceApp, NugetFeed feed)
        {
            return new UninstallService(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetUninstallPackageOp(string targetPath, ServiceApp serviceApp, NugetFeed feed)
        {
            return new UninstallPackage(targetPath, serviceApp, feed);
        }
    }
}
