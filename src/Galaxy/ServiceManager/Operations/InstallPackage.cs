using NuGet;
using System.IO;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallPackage: ServiceOperation
    {
        public InstallPackage(string targetPath, Deployment deployment, NugetFeed feed):
            base(targetPath, deployment, feed)
        {

        }

        public override void Execute()
        {
            string packageID = _feed.Name;

            string serviceTargetPath = Path.Combine(_targetPath, Deployment.DisplayName);

            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(_feed.Uri);

            PackageManager packageManager = new PackageManager(repo, serviceTargetPath);

            packageManager.InstallPackage(packageID, new SemanticVersion(_deployment.PackageVersion));
        }
    }
}
