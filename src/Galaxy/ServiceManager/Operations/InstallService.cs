using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallService : WinServiceOperation
    {
        public InstallService(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {
        }

        public override void Execute()
        {
            string serviceTargetPath = Path.Combine(_targetPath, Deployment.DisplayName);

            string exePath = Path.Combine(serviceTargetPath, ServiceHostFileName);

            string exeParams = string.Format("install");

            ProcessStarter.ExecuteWithParams(exePath, exeParams);
        }
    }
}