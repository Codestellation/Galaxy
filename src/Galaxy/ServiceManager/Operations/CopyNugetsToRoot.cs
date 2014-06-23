using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class CopyNugetsToRoot: ServiceOperation
    {
        public CopyNugetsToRoot(string targetPath, ServiceApp serviceApp, NugetFeed feed) :
            base(targetPath, serviceApp, feed)
        {

        }

        public override void Execute()
        {
            try
            {
                string serviceTargetPath = Path.Combine(_targetPath, _serviceApp.DisplayName);

                var packageFolders = Directory.EnumerateDirectories(serviceTargetPath);

                foreach (var packagePath in packageFolders)
                {
                    CopyDirectoryHelper.DirectoryCopy(packagePath, serviceTargetPath, true);

                    Directory.Delete(packagePath, true);
                }

                StoreResult(OperationResult.OR_OK, "");
            }
            catch (System.Exception ex)
            {
                StoreResult(OperationResult.OR_FAIL, ex.Message);
            }
        }
    }
}
