using System.IO;
using Codestellation.Galaxy.Domain;
using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StopService: ServiceOperation
    {
        public StopService(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }

        void DoStopService()
        {
            using(ServiceController sc = new ServiceController(Deployment.ServiceName))
            {
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped);
            }
        }

        public override void Execute()
        {
            try
            {
                DoStopService();

                StoreResult(this, OperationResultType.OR_OK, "");
            }
            catch (System.Exception ex)
            {
                StoreResult(this, OperationResultType.OR_FAIL, ex.Message);
            }
        }
    }
}
