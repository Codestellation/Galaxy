using System;
using System.IO;
using Codestellation.Galaxy.Domain;


namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallService: WinServiceOperation
    {
        public UninstallService(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {
            
        }

        public override void Execute()
        {
            if(IsServiceExists(_deployment.ServiceName))
            {
                string serviceTargetPath = Path.Combine(_targetPath, _deployment.DisplayName);

                string exePath = Path.Combine(serviceTargetPath, ServiceHostFileName);

                string exeParams = "uninstall";

                int resultCode = 0;
                if ((resultCode = ExecuteWithParams(exePath, exeParams)) != 0)
                {
                    throw new InvalidOperationException(string.Format("execution of {0} with params {1} returned {2}", exePath, exeParams, resultCode));
                }
            }
        }
    }
}
