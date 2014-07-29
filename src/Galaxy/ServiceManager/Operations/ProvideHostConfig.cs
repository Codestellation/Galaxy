using System.IO;
using Codestellation.Galaxy.ServiceManager.Helpers;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class ProvideHostConfig  : IOperation
    {
        private readonly string _serviceFolder;
        private readonly ServiceConfig _config;
        private const string ServiceConfigFileName = "service-config.xml";

        public ProvideHostConfig(string serviceFolder, ServiceConfig config)
        {
            _serviceFolder = serviceFolder;
            _config = config;
        }

        public void Execute(TextWriter buildLog)
        {
            var serviceConfigFileNameFull = Path.Combine(_serviceFolder, ServiceConfigFileName);

            _config.Serialize(serviceConfigFileNameFull);
        }
    }
}
