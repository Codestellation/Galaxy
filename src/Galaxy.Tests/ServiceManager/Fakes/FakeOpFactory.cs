using Codestellation.Galaxy.ServiceManager;

namespace Codestellation.Galaxy.Tests.ServiceManager.Fakes
{
    public class FakeOpFactory: IOperationsFactory
    {
        public ServiceOperation GetInstallPackageOp(string targetPath, Domain.Deployment deployment, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, deployment, feed);
        }

        public ServiceOperation GetCopyNugetsToRootOp(string targetPath, Domain.Deployment deployment, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, deployment, feed);
        }

        public ServiceOperation GetProvideServiceConfigOp(string targetPath, Domain.Deployment deployment, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, deployment, feed);
        }

        public ServiceOperation GetInstallServiceOp(string targetPath, Domain.Deployment deployment, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, deployment, feed);
        }

        public ServiceOperation GetStartServiceOp(string targetPath, Domain.Deployment deployment, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, deployment, feed);
        }

        public ServiceOperation GetStopServiceOp(string targetPath, Domain.Deployment deployment, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, deployment, feed);
        }

        public ServiceOperation GetUninstallServiceOp(string targetPath, Domain.Deployment deployment, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, deployment, feed);
        }

        public ServiceOperation GetUninstallPackageOp(string targetPath, Domain.Deployment deployment, Domain.NugetFeed feed)
        {
            return new FakeOpSuccess(targetPath, deployment, feed);
        }

    }
}
