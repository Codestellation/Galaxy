using System.Linq;
using System.ServiceProcess;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public abstract class WinServiceOperation: OperationBase
    {
        public WinServiceOperation(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }


        protected int ExecuteWithParams(string exePath, string exeParams)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = exePath;
            startInfo.Arguments = exeParams;
            startInfo.Verb = "runas";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            return process.ExitCode;
        }


        protected bool IsServiceExists(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return services.Any(item => item.ServiceName == serviceName);
        }
    }
}
