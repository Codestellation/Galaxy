using System.IO;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployConsulConfig : IOperation
    {
        private readonly string _serviceFolder;

        public DeployConsulConfig(string serviceFolder)
        {
            _serviceFolder = serviceFolder;
        }

        public void Execute(DeploymentTaskContext context)
        {
            string consulName;
            if (context.TryGetValue(DeploymentTaskContext.ConsulName, out consulName))
            {
                var address = context.GetValue<string>(DeploymentTaskContext.ConsulAddress);
                CreateConsulConfig(context.BuildLog, consulName, address);
            }
            else
            {
                context.BuildLog.Write("Consul name not found, consul config deployment skipped.");
            }
        }

        private void CreateConsulConfig(TextWriter buildLog, string consulName, string address)
        {
            buildLog.WriteLine("Consul name is '{0}'", consulName);
            var config = new { Name = consulName, Address = address };

            var configPath = Path.Combine(_serviceFolder, "consul.json");
            var serializedConfig = JsonConvert.SerializeObject(config);
            buildLog.WriteLine("Consul config is '{0}'", serializedConfig);
            buildLog.WriteLine("Write consul config to '{0}'", configPath);
            File.WriteAllText(configPath, serializedConfig);
            buildLog.WriteLine("Consul config successfully written");
        }
    }
}