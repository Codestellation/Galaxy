using NuGet;
using System.IO;
using Codestellation.Galaxy.Domain;
using System;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallPackage: OperationBase
    {
        public InstallPackage(string targetPath, Deployment deployment, NugetFeed feed):
            base(targetPath, deployment, feed)
        {

        }

        public override void Execute()
        {
            string packageID = _deployment.PackageName;

            string serviceTargetPath = Path.Combine(_targetPath, _deployment.DisplayName);

            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(_feed.Uri);

            PackageManager packageManager = new PackageManager(repo, serviceTargetPath);

            if (_deployment.PackageVersion == null)
            {
                throw new ArgumentException(string.Format("No package version was specified for deployment {0}", _deployment.DisplayName));
            }

            packageManager.InstallPackage(packageID, new SemanticVersion(_deployment.PackageVersion));
        }
    }
}
