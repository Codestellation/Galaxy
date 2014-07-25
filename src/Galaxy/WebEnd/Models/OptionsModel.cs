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
            HostPackageId = options.HostPackageId;
            HostPackageFeedUri = options.HostPackageFeedUri;
        }

        [Display(Name = "Applications Folder", Prompt = "Path. Leave blank to use default")]
        public string DeployFolder { get; set; }
        
        [Display(Name = "Host package", Prompt = "PackageId")]
        public string HostPackageId { get; set; }

        [Display(Name = "Host package feed", Prompt = "Feed Uri")]
        public string HostPackageFeedUri { get; set; }

        public void Update(Options options)
        {
            options.RootDeployFolder = DeployFolder;
            options.HostPackageId = HostPackageId;
            options.HostPackageFeedUri = HostPackageFeedUri;
        }
    }
}