using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager
{
    public interface IOperationsFactory
    {
        ServiceOperation GetInstallPackageOp(string targetPath, ServiceApp serviceApp, NugetFeed feed);
        ServiceOperation GetCopyNugetsToRootOp(string targetPath, ServiceApp serviceApp, NugetFeed feed);
        ServiceOperation GetProvideServiceConfigOp(string targetPath, ServiceApp serviceApp, NugetFeed feed);
        ServiceOperation GetInstallServiceOp(string targetPath, ServiceApp serviceApp, NugetFeed feed);
        ServiceOperation GetStartServiceOp(string targetPath, ServiceApp serviceApp, NugetFeed feed);
        ServiceOperation GetStopServiceOp(string targetPath, ServiceApp serviceApp, NugetFeed feed);
        ServiceOperation GetUninstallServiceOp(string targetPath, ServiceApp serviceApp, NugetFeed feed);
        ServiceOperation GetUninstallPackageOp(string targetPath, ServiceApp serviceApp, NugetFeed feed);
    }
}
