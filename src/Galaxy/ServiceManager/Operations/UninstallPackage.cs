using System;
using System.IO;
using System.Text;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallPackage: OperationBase
    {
        public UninstallPackage(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {

        }

        public override void Execute(StringBuilder buildLog)
        {

            if (!Directory.Exists(ServiceFolder))
            {
                throw new InvalidOperationException("uninstall unavaliable: run install first");
            }

            Directory.Delete(ServiceFolder, true);
        }
    }
}
