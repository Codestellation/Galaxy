using System;
using System.IO;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallService: WinServiceOperation
    {
        public InstallService(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }

        public override void Execute()
        {
            string serviceTargetPath = Path.Combine(_targetPath, Deployment.DisplayName);

            string exePath = Path.Combine(serviceTargetPath, ServiceHostFileName);

            string exeParams = string.Format("install -servicename \"{0}\"",
                    Deployment.ServiceName);

            int resultCode;
            if ((resultCode = ExecuteWithParams(exePath, exeParams)) != 0)
            {
                throw new InvalidOperationException(string.Format("execution of {0} with params {1} returned {2}", exePath, exeParams, resultCode));
            }
        }
    }
}
