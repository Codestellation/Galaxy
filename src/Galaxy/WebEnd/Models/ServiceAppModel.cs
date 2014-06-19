using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Nejdb.Bson;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class ServiceAppModel
    {
        public string Id { get; set; }
        public bool IsNew { get; set; }
        [Display(Name = "Service name", Prompt = "Service name")]
        public string ServiceName { get; set; }
        [Display(Name = "Display name", Prompt = "Display name")]
        public string DisplayName { get; set; }
        public string Description { get; set; }
        [Display(Name = "Feed name", Prompt = "Feed name")]
        public string FeedName { get; set; }
        [Display(Name = "Assembly-qualified type name", Prompt = "Assembly-qualified type name")]
        public string AssemblyQualifiedType { get; set; }
        [Display(Name = "Package name", Prompt = "Package name")]
        public string PackageName { get; set; }

        readonly IEnumerable<string> _avaliableFeeds;
        public IEnumerable<string> AvaliableFeeds { get { return _avaliableFeeds; } }

        public ServiceAppModel()
        {
            IsNew = true;
            Id = new ObjectId().ToString();
        }

        public ServiceAppModel(IEnumerable<string> avaliableFeeds)
        {
            IsNew = true;
            Id = new ObjectId().ToString();
            _avaliableFeeds = avaliableFeeds;
        }

        public ServiceAppModel(ServiceApp serviceApp, IEnumerable<string> avaliableFeeds)
        {
            Id = serviceApp.Id.ToString();

            this.DisplayName = serviceApp.DisplayName;
            this.ServiceName = serviceApp.ServiceName;
            this.AssemblyQualifiedType = serviceApp.AssemblyQualifiedType;
            this.Description = serviceApp.Description;
            this.FeedName = serviceApp.FeedName;
            this.PackageName = serviceApp.PackageName;
            _avaliableFeeds = avaliableFeeds;

            IsNew = false;
        }

        public void Update(ServiceApp serviceApp)
        {
            serviceApp.DisplayName = DisplayName;
            serviceApp.ServiceName = ServiceName;
            serviceApp.AssemblyQualifiedType = AssemblyQualifiedType;
            serviceApp.PackageName = PackageName;
            serviceApp.Description = Description;
            serviceApp.FeedName = FeedName;
        }

        public ServiceApp ToServiceApp()
        {
            var serviceApp = new ServiceApp
            {
                Id = new ObjectId(Id)
            };

            Update(serviceApp);
            return serviceApp;
        }
    }
}
