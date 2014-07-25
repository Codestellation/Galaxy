using System.ServiceProcess;
using System.Text;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StartService: WinServiceOperation
    {
        public StartService(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {

        }

        public override void Execute(StringBuilder buildLog)
        {
            using (ServiceController sc = new ServiceController(Deployment.GetServiceName()))
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.StartPending);
            }
        }
    }
}
