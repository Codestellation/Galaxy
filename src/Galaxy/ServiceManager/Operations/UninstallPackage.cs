using System.IO;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallPackage: ServiceOperation
    {
        public UninstallPackage(string targetPath, ServiceApp serviceApp, NugetFeed feed) :
            base(targetPath, serviceApp, feed)
        {

        }
        public override void Execute()
        {
            try
            {
                string serviceTargetPath = Path.Combine(_targetPath, _serviceApp.DisplayName);

                if (!Directory.Exists(serviceTargetPath))
                {
                    StoreResult(OperationResult.OR_FAIL, "uninstall unavaliable: run install first");
                    return;
                }

                Directory.Delete(serviceTargetPath, true);

                StoreResult(OperationResult.OR_OK, "");
            }
            catch (System.Exception ex)
            {
                StoreResult(OperationResult.OR_FAIL,
                            string.Format("uninstall error: {0}", ex.Message));
            }            
        }
    }
}
