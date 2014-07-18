using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallService : WinServiceOperation
    {
        public UninstallService(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {
        }

        public override void Execute()
        {
            if (!IsServiceExists(_deployment.GetServiceName()))
            {
                return;
            }

            var serviceTargetPath = Path.Combine(_targetPath, _deployment.DisplayName);

            var exePath = Path.Combine(serviceTargetPath, ServiceHostFileName);

            var exeParams = "uninstall";

            ProcessStarter.ExecuteWithParams(exePath, exeParams);
        }
    }
}
