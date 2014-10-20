using System.IO;
using Codestellation.Quarks.IO;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Configuration
{
    public class ServiceConfig
    {
        public static readonly string DefaultFilePath = Folder.Combine("data", "config.json");

        public int WebPort;

        public string InstanceName;

        public static ServiceConfig Read()
        {
            var config = File.ReadAllText(DefaultFilePath);

            return JsonConvert.DeserializeObject<ServiceConfig>(config);
        }

        public void Save()
        {
            var config = JsonConvert.SerializeObject(this, Formatting.Indented);
            var folder = Path.GetDirectoryName(DefaultFilePath);
            Folder.EnsureExists(folder);
            File.WriteAllText(DefaultFilePath, config);
        }
    }
}