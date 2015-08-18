using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using Codestellation.Quarks.Enumerations;
using TimeoutException = System.TimeoutException;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StopService : WinServiceOperation
    {
        private DeploymentTaskContext _context;
        private Process _process;
        private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(1);

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

            TryFindProcess();

            _context.SetValue(DeploymentTaskContext.ServiceStatus, status);

            if (status == ServiceControllerStatus.Stopped || status == ServiceControllerStatus.StopPending)
            {
                _context.BuildLog.WriteLine("Service '{0}' at state '{1}'. Stop skipped.", ServiceName, status.AsString());
            }
            else
            {
                _context.BuildLog.WriteLine("Stopping service '{0}'", ServiceName);
                sc.Stop();
            }

            sc.WaitForStatus(ServiceControllerStatus.Stopped, Timeout);

            WaitForProcessExit();
        }

        private void WaitForProcessExit()
        {
            if (_process == null)
            {
                return;
            }

            var hasExited = _process.WaitForExit((int)Timeout.TotalMilliseconds);

            if (hasExited)
            {
                return;
            }
            var message = string.Format(
                "Process {0}-{1} did not exit after period {2}. Possible hang up?.", _process.Id, _process.ProcessName, Timeout);
            throw new TimeoutException(message);
        }

        private void TryFindProcess()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service WHERE Name = '" + ServiceName + "'");

            var managementObject = searcher.Get().Cast<ManagementObject>().SingleOrDefault();
            if (managementObject == null)
            {
                return;
            }

            var processId = Convert.ToInt32(managementObject["ProcessId"]);
            if (processId == 0)
            {
                return;
            }

            _process = Process.GetProcessById(processId);
        }
    }
}