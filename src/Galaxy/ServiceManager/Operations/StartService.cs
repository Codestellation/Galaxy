using System.IO;
using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class StartService : WinServiceOperation
    {
        public StartService(string serviceName)
            : base(serviceName)
        {

        }

        public override void Execute(TextWriter buildLog)
        {
            buildLog.WriteLine("Starting service {0}", ServiceName);

            Execute(sc =>
            {
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.StartPending);
            });
        }
    }
}
