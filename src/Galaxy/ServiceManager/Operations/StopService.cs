using System.IO;
using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StopService : WinServiceOperation
    {
        private TextWriter _buildLog;

        public StopService(string serviceName)
            : base(serviceName)
        {

        }

        public override void Execute(DeploymentTaskContext context)
        {
            _buildLog = context.BuildLog;

            Execute(StopServiceAction);
        }

        private void StopServiceAction(ServiceController sc)
        {
            var status = sc.Status;
            if (status == ServiceControllerStatus.Stopped || status == ServiceControllerStatus.StopPending)
            {
                _buildLog.WriteLine("Service '{0}' at state '{1}'. Stop skipped.", ServiceName, status.ToString());
            }
            else
            {
                _buildLog.WriteLine("Stopping service '{0}'", ServiceName);
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped);
            }

        }
    }
}
