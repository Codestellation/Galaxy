using System.Dynamic;
using System.IO;
using Codestellation.Galaxy.Domain;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployHostConfig : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            var deployFolder = context.GetValue<ServiceFolders>(DeploymentTaskContext.Folders).DeployFolder;

            dynamic hostConfig = new ExpandoObject();

            var hostConfigString = JsonConvert.SerializeObject(hostConfig, Formatting.Indented);

            context.BuildLog.WriteLine("Generated host config");
            context.BuildLog.WriteLine(hostConfigString);

            WriteConfig(context.BuildLog, hostConfigString, (string)deployFolder);
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