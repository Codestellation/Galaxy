using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StartService : WinServiceOperation
    {
        public StartService(string serviceName)
            : base(serviceName)
        {

        }

        public override void Execute(DeploymentTaskContext context)
        {
            context.BuildLog.WriteLine("Starting service {0}", ServiceName);

            Execute(sc =>
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.StartPending);
            });
        }
    }
}
