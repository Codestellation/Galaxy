using System.Text;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallService : WinServiceOperation
    {
        public InstallService(string basePath, Deployment deployment) :
            base(basePath, deployment)
        {
        }

        public override void Execute(StringBuilder buildLog)
        {
            string exePath = Path.Combine(ServiceFolder, ServiceHostFileName);

            string exeParams = string.Format("install");

            ProcessStarter.ExecuteWithParams(exePath, exeParams);
        }
    }
}