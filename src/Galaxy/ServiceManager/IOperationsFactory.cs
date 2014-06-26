using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager
{
    public interface IOperationsFactory
    {
        ServiceOperation GetInstallPackageOp(string targetPath, Deployment deployment, NugetFeed feed);
        ServiceOperation GetCopyNugetsToRootOp(string targetPath, Deployment deployment, NugetFeed feed);
        ServiceOperation GetProvideServiceConfigOp(string targetPath, Deployment deployment, NugetFeed feed);
        ServiceOperation GetInstallServiceOp(string targetPath, Deployment deployment, NugetFeed feed);
        ServiceOperation GetStartServiceOp(string targetPath, Deployment deployment, NugetFeed feed);
        ServiceOperation GetStopServiceOp(string targetPath, Deployment deployment, NugetFeed feed);
        ServiceOperation GetUninstallServiceOp(string targetPath, Deployment deployment, NugetFeed feed);
        ServiceOperation GetUninstallPackageOp(string targetPath, Deployment deployment, NugetFeed feed);
    }
}
