using System.Text;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallService : WinServiceOperation
    {
        public UninstallService(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {
        }

        public override void Execute(StringBuilder buildLog)
        {
            if (!IsServiceExists(Deployment.GetServiceName()))
            {
                return;
            }

            var exePath = Path.Combine(ServiceFolder, ServiceHostFileName);

            var exeParams = "uninstall";

            ProcessStarter.ExecuteWithParams(exePath, exeParams);
        }
    }
}
