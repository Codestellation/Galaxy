using System.IO;
using Codestellation.Galaxy.Domain;
using System;


namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallService: ServiceOperation
    {
        public UninstallService(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {
            
        }

        public override void Execute()
        {
            string serviceTargetPath = Path.Combine(_targetPath, Deployment.DisplayName);

            string exePath = Path.Combine(serviceTargetPath, serviceHostFileName);

            string exeParams = "uninstall";

            int resultCode = 0;
            if ((resultCode = ExecuteWithParams(exePath, exeParams)) != 0)
            {
                throw new InvalidOperationException(string.Format("execution of {0} with params {1} returned {2}", exePath, exeParams, resultCode));
            }
        }
    }
}
