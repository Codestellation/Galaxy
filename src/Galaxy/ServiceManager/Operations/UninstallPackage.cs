using System;
using System.IO;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallPackage: OperationBase
    {
        public UninstallPackage(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }
        public override void Execute()
        {
            string serviceTargetPath = Path.Combine(_targetPath, Deployment.DisplayName);

            if (!Directory.Exists(serviceTargetPath))
            {
                throw new InvalidOperationException("uninstall unavaliable: run install first");
            }

            Directory.Delete(serviceTargetPath, true);
        }
    }
}
