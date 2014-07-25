using System.Text;
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

        public override void Execute(StringBuilder buildLog)
        {
            string packageId = Deployment.PackageId;

            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(Feed.Uri);

            PackageManager packageManager = new PackageManager(repo, ServiceFolder);

            if (Deployment.PackageVersion == null)
            {
                throw new ArgumentException(string.Format("No package version was specified for deployment {0}", Deployment.DisplayName));
            }

            packageManager.InstallPackage(packageId, new SemanticVersion(Deployment.PackageVersion));
        }
    }
}
