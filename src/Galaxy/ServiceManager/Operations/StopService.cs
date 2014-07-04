using System.Linq;
using System.ServiceProcess;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StopService : WinServiceOperation
    {
        public StopService(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }

        public override void Execute()
        {
            if(IsServiceExists(Deployment.ServiceName))
            {
                using (ServiceController sc = new ServiceController(Deployment.ServiceName))
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
