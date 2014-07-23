using NuGet;
using Codestellation.Galaxy.Domain;
using System;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallPackage: OperationBase
    {
        public InstallPackage(string basePath, Deployment deployment, NugetFeed feed):
            base(basePath, deployment, feed)
        {

        }

        public override void Execute()
        {
            string packageId = _deployment.PackageId;

            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(_feed.Uri);

            PackageManager packageManager = new PackageManager(repo, ServiceFolder);

            if (_deployment.PackageVersion == null)
            {
                throw new ArgumentException(string.Format("No package version was specified for deployment {0}", _deployment.DisplayName));
            }

            packageManager.InstallPackage(packageId, new SemanticVersion(_deployment.PackageVersion));
        }
    }
}
