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
            try
            {
                string packageID = _feed.Name;

                string serviceTargetPath = Path.Combine(_targetPath, Deployment.DisplayName);

                IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(_feed.Uri);

                PackageManager packageManager = new PackageManager(repo, serviceTargetPath);

                packageManager.InstallPackage(packageID, new SemanticVersion(_deployment.PackageVersion));

                StoreResult(this, OperationResultType.OR_OK, "install succeeded");
            }
            catch (System.Exception ex)
            {
                StoreResult(this, OperationResultType.OR_FAIL,
                            string.Format("install error: {0}", ex.Message));
            }            
        }
    }
}
