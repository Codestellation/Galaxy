using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StopService : WinServiceOperation
    {
        private DeploymentTaskContext _context;

        public StopService(string serviceName)
            : base(serviceName)
        {

        }

        public override void Execute(DeploymentTaskContext context)
        {
            _context = context;

            Execute(StopServiceAction);
        }

        private void StopServiceAction(ServiceController sc)
        {
            var status = sc.Status;

            _context.SetValue(DeploymentTaskContext.ServiceStatus, status);

            if (status == ServiceControllerStatus.Stopped || status == ServiceControllerStatus.StopPending)
            {
                _context.BuildLog.WriteLine("Service '{0}' at state '{1}'. Stop skipped.", ServiceName, status.ToString());
            }
            else
            {
                _context.BuildLog.WriteLine("Stopping service '{0}'", ServiceName);
                sc.Stop();
            }

            sc.WaitForStatus(ServiceControllerStatus.Stopped);
        }
    }
}
