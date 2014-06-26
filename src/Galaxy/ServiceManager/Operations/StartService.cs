using System.IO;
using Codestellation.Galaxy.Domain;
using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StartService: ServiceOperation
    {
        public StartService(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }

        void DoStartService()
        {
            using (ServiceController sc = new ServiceController(Deployment.ServiceName))
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.StartPending);
            }
        }

        public override void Execute()
        {
            try
            {
                DoStartService();

                StoreResult(OperationResult.OR_OK, "");
            }
            catch (System.Exception ex)
            {
                StoreResult(OperationResult.OR_FAIL, ex.Message);
            }
        }
    }
}
