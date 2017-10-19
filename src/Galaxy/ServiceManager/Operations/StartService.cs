using System;
using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StartService : WinServiceOperation
    {
        protected override void ExecuteInternal(DeploymentTaskContext context)
        {
            var startService =
                context.ServiceStatus == ServiceControllerStatus.Running
                || context.ServiceStatus == ServiceControllerStatus.StartPending
                || ForceStartService(context.Parameters);

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

        public bool ForceStartService(dynamic parameters)
        {
            try
            {
                return parameters.ForceStartService;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}