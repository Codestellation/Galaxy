using System.ServiceProcess;
using System.Text;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StopService : WinServiceOperation
    {
        public StopService(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {

        }

        public override void Execute(StringBuilder buildLog)
        {
            if (IsServiceExists(Deployment.ServiceName))
            {
                using (ServiceController sc = new ServiceController(Deployment.GetServiceName()))
                {
                    if(sc.Status != ServiceControllerStatus.Stopped)
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                }
            
            } 
        }

    }
}
