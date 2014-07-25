using System.IO;
using System.ServiceProcess;
using System.Text;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StartService: WinServiceOperation
    {
        public StartService(string basePath, Deployment deployment) :
            base(basePath, deployment)
        {

        }

        public override void Execute(TextWriter buildLog)
        {
            using (ServiceController sc = new ServiceController(Deployment.GetServiceName()))
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.StartPending);
            }
        }
    }
}
