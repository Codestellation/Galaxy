using System.ServiceProcess;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StartService: WinServiceOperation
    {
        public StartService(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }

        public override void Execute()
        {
            using (ServiceController sc = new ServiceController(Deployment.ServiceName))
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.StartPending);
            }
        }
    }
}
