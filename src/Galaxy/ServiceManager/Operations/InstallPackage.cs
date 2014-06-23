using NuGet;
using System.IO;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallPackage: ServiceOperation
    {
        public InstallPackage(string targetPath, ServiceApp serviceApp, NugetFeed feed):
            base(targetPath, serviceApp, feed)
        {

        }

        public override void Execute()
        {
            try
            {
                string packageID = _feed.Name;

                string serviceTargetPath = Path.Combine(_targetPath, _serviceApp.DisplayName);

                IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(_feed.Uri);

                PackageManager packageManager = new PackageManager(repo, serviceTargetPath);

                packageManager.InstallPackage(packageID);

                StoreResult(OperationResult.OR_OK, "install succeeded");
            }
            catch (System.Exception ex)
            {
                StoreResult(OperationResult.OR_FAIL,
                            string.Format("install error: {0}", ex.Message));
            }            
        }
    }
}
