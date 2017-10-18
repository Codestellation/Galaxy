using System.Dynamic;
using System.IO;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployHostConfig : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            dynamic hostConfig = new ExpandoObject();

            var hostConfigString = JsonConvert.SerializeObject(hostConfig, Formatting.Indented);

            context.BuildLog.WriteLine("Generated host config");
            context.BuildLog.WriteLine(hostConfigString);

            WriteConfig(context.BuildLog, hostConfigString, (string)context.Folders.DeployFolder);
        }

        private void WriteConfig(TextWriter buildLog, string serviceFolder, string hostConfig)
        {
            var configPath = Path.Combine(serviceFolder, "host.config.json");

            buildLog.WriteLine("Write host config to '{0}'", configPath);
            File.WriteAllText(configPath, hostConfig);
            buildLog.WriteLine("Config successfully written");
        }
    }
}