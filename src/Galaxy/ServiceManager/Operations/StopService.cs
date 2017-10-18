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
        private Process _process;
        private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(1);

        protected override void ExecuteInternal(DeploymentTaskContext context)
        {
            Execute(StopServiceAction);
        }

        private void StopServiceAction(ServiceController sc)
        {
            var status = sc.Status;

            TryFindProcess();

            Context.SetValue(DeploymentTaskContext.ServiceStatus, status);

            if (status == ServiceControllerStatus.Stopped || status == ServiceControllerStatus.StopPending)
            {
                Context.BuildLog.WriteLine("Service '{0}' at state '{1}'. Stop skipped.", ServiceName, status.AsString());
            }
            else
            {
                Context.BuildLog.WriteLine("Stopping service '{0}'", ServiceName);
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
            var message = $"Process {_process.Id}-{_process.ProcessName} did not exit after period {Timeout}. Possible hang up?.";
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