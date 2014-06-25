using System.IO;
using Codestellation.Galaxy.Domain;
using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StopService: ServiceOperation
    {
        public StopService(string targetPath, ServiceApp serviceApp, NugetFeed feed) :
            base(targetPath, serviceApp, feed)
        {

        }

        void DoStopService()
        {
            using(ServiceController sc = new ServiceController(_serviceApp.ServiceName))
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

                StoreResult(OperationResult.OR_OK, "");
            }
            catch (System.Exception ex)
            {
                StoreResult(OperationResult.OR_FAIL, ex.Message);
            }
        }
    }
}
