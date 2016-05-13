using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using Codestellation.Galaxy.Domain;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployHostConfig : IOperation
    {
        private readonly string _serviceFolder;

        public DeployHostConfig(string serviceFolder)
        {
            _serviceFolder = serviceFolder;
        }

        public void Execute(DeploymentTaskContext context)
        {
            dynamic hostConfig = new ExpandoObject();

            foreach (var folder in context.GetValue<SpecialFolder[]>(DeploymentTaskContext.Folders))
            {
                ((IDictionary<string, object>)hostConfig)[folder.Purpose] = folder.FullPath;
            }

            var hostConfigString = JsonConvert.SerializeObject(hostConfig, Formatting.Indented);

            context.BuildLog.WriteLine("Generated host config");
            context.BuildLog.WriteLine(hostConfigString);

            WriteConfig(context.BuildLog, hostConfigString);
        }

        private void WriteConfig(TextWriter buildLog, string hostConfig)
        {
            var configPath = Path.Combine(_serviceFolder, "host.config.json");

            buildLog.WriteLine("Write host config to '{0}'", configPath);
            File.WriteAllText(configPath, hostConfig);
            buildLog.WriteLine("Config successfully written");
        }
    }
}