using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StartService : WinServiceOperation
    {
        protected override void ExecuteInternal(DeploymentTaskContext context)
        {
            var startService = false;
            if (context.TryGetValue(DeploymentTaskContext.ServiceStatus, out ServiceControllerStatus status))
            {
                startService = status == ServiceControllerStatus.Running ||
                    status == ServiceControllerStatus.StartPending;
            }

            if (context.TryGetValue(DeploymentTaskContext.ForceStartService, out bool forceStart))
            {
                startService = startService || forceStart;
            }

            if (startService)
            {
                context.BuildLog.WriteLine("Starting service {0}", ServiceName);
                Execute(
                    sc =>
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