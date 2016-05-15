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

            ConfigsFolder = options.FolderOptions.Configs;
            LogsFolder = options.FolderOptions.Logs;
            DataFolder = options.FolderOptions.Data;
        }

        [Display(Name = "Applications Folder", Prompt = "Path. Leave blank to use default")]
        public string DeployFolder { get; set; }

        [Display(Name = "Configurations Folder")]
        public string ConfigsFolder { get; set; }

        [Display(Name = "Logs Folder")]
        public string LogsFolder { get; set; }

        [Display(Name = "Data Folder")]
        public string DataFolder { get; set; }

        public void Update(Options options)
        {
            options.RootDeployFolder = DeployFolder;

            options.FolderOptions.Configs = ConfigsFolder;
            options.FolderOptions.Logs = LogsFolder;
            options.FolderOptions.Data = DataFolder;
        }
    }
}