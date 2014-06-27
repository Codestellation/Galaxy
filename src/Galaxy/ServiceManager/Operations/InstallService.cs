using System.IO;
using Codestellation.Galaxy.Domain;


namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallService: ServiceOperation
    {
        public InstallService(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }

        public override void Execute()
        {
            try
            {
                string serviceTargetPath = Path.Combine(_targetPath, Deployment.DisplayName);

                string exePath = Path.Combine(serviceTargetPath, serviceHostFileName);

                string exeParams = string.Format("install -servicename \"{0}\"",
                        Deployment.ServiceName);

                int resultCode = 0;
                if ((resultCode = ExecuteWithParams(exePath, exeParams)) != 0)
                {
                    StoreResult(this, OperationResultType.OR_FAIL,
                                string.Format("execution of {0} with params {1} returned {2}", exePath, exeParams, resultCode));
                    return;
                }

                StoreResult(this, OperationResultType.OR_OK, "");
            }
            catch (System.Exception ex)
            {
                StoreResult(this, OperationResultType.OR_FAIL, ex.Message);
            }
        }
    }
}
