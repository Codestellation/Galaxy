using System.IO;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallPackage: ServiceOperation
    {
        public UninstallPackage(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }
        public override void Execute()
        {
            try
            {
                string serviceTargetPath = Path.Combine(_targetPath, Deployment.DisplayName);

                if (!Directory.Exists(serviceTargetPath))
                {
                    StoreResult(this, OperationResultType.OR_FAIL, "uninstall unavaliable: run install first");
                    return;
                }

                Directory.Delete(serviceTargetPath, true);

                StoreResult(this, OperationResultType.OR_OK, "");
            }
            catch (System.Exception ex)
            {
                StoreResult(this, OperationResultType.OR_FAIL,
                            string.Format("uninstall error: {0}", ex.Message));
            }            
        }
    }
}
