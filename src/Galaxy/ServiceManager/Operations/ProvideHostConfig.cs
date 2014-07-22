using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class ProvideHostConfig: OperationBase
    {
        private const string ServiceConfigFileName = "service-config.xml";

        public ProvideHostConfig(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {

        }
        public override void Execute()
        {
            string serviceConfigFileNameFull = Path.Combine(ServiceFolder, ServiceConfigFileName);

            var config = new ServiceConfig(Deployment);

            config.Serialize(serviceConfigFileNameFull);
        }
    }
}
