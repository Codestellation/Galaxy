using System.IO;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StartService: ServiceOperation
    {
        public StartService(string targetPath, ServiceApp serviceApp, NugetFeed feed) :
            base(targetPath, serviceApp, feed)
        {

        }
        public override void Execute()
        {
            try
            {
                string serviceTargetPath = Path.Combine(_targetPath, _serviceApp.DisplayName);

                string exePath = Path.Combine(serviceTargetPath, serviceHostFileName);

                string exeParams = "start";

                int resultCode = 0;
                if ((resultCode = ExecuteWithParams(exePath, exeParams)) != 0)
                {
                    StoreResult(OperationResult.OR_FAIL,
                                string.Format("execution of {0} with params {1} returned {2}", exePath, exeParams, resultCode));
                    return;
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
