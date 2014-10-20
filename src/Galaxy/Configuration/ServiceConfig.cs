using System.IO;
using Codestellation.Quarks.IO;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Configuration
{
    public class ServiceConfig
    {
        public int WebPort;

        public string InstanceName;

        public static ServiceConfig Read(string fileName = null)
        {
            var configFileName = string.IsNullOrWhiteSpace(fileName) 
                ? "config.json" 
                : fileName;
            
            var filePath =  Path.IsPathRooted(configFileName) 
                ? configFileName
                : Folder.Combine("data", configFileName);

            var config = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<ServiceConfig>(config);
        } 
    }
}