using System.IO;
using Codestellation.Galaxy.Domain;
using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StartService: ServiceOperation
    {
        public StartService(string targetPath, ServiceApp serviceApp, NugetFeed feed) :
            base(targetPath, serviceApp, feed)
        {

        }

        void DoStartService()
        {
            using (ServiceController sc = new ServiceController(_serviceApp.ServiceName))
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
