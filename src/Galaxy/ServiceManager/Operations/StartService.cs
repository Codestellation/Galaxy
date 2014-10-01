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
            var startService = false;
            ServiceControllerStatus status;
            if (context.TryGetValue(DeploymentTaskContext.ServiceStatus, out status))
            {
                startService = status == ServiceControllerStatus.Running ||
                               status == ServiceControllerStatus.StartPending;
            }

            bool forceStart;
            if (context.TryGetValue(DeploymentTaskContext.ForceStartService, out forceStart))
            {
                startService = startService || forceStart;
            }


            if (startService)
            {
                context.BuildLog.WriteLine("Starting service {0}", ServiceName);
                Execute(sc =>
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.StartPending);
                });
            }
            else
            {
                context.BuildLog.WriteLine("Service {0} was not started before update. Start skipped.", ServiceName);
            }
        }
    }
}
