using System.ComponentModel.DataAnnotations;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class OptionsModel
    {
        public OptionsModel()
        {
            //note: nancy binder require default constructor
        }

        public OptionsModel(Options options)
        {
            DeployFolder = options.RootDeployFolder;
            PurgeLogsOlderThan = options.PurgeLogsOlderThan;
            ConsulAddress = options.ConsulAddress;
        }

        [Display(Name = "Applications Folder", Prompt = "Path. Leave blank to use default")]
        public string DeployFolder { get; set; }

        [Display(Name = "Purge log older then", Prompt = "days")]
        public int PurgeLogsOlderThan { get; set; }

        [Display(Name = "Consul Address", Prompt = "host:port Leave blank to use default")]
        public string ConsulAddress { get; set; }

        public void Update(Options options)
        {
            options.RootDeployFolder = DeployFolder;
            options.PurgeLogsOlderThan = PurgeLogsOlderThan;
            options.ConsulAddress = ConsulAddress;
        }
    }
}