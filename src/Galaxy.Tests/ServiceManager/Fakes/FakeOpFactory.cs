using Codestellation.Galaxy.ServiceManager;

namespace Codestellation.Galaxy.Tests.ServiceManager.Fakes
{
    public class FakeOpFactory: IOperationsFactory
    {
        public ServiceOperation GetInstallPackageOp(string targetPath, Domain.ServiceApp serviceApp, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetCopyNugetsToRootOp(string targetPath, Domain.ServiceApp serviceApp, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetProvideServiceConfigOp(string targetPath, Domain.ServiceApp serviceApp, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetInstallServiceOp(string targetPath, Domain.ServiceApp serviceApp, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetStartServiceOp(string targetPath, Domain.ServiceApp serviceApp, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetStopServiceOp(string targetPath, Domain.ServiceApp serviceApp, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetUninstallServiceOp(string targetPath, Domain.ServiceApp serviceApp, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, serviceApp, feed);
        }

        public ServiceOperation GetUninstallPackageOp(string targetPath, Domain.ServiceApp serviceApp, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, serviceApp, feed);
        }

    }
}
