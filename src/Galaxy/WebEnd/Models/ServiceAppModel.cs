using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nejdb.Bson;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class ServiceAppModel
    {
        public ObjectId Id { get; set; }
        public bool IsNew { get; set; }
        [Display(Name = "Service name", Prompt = "Service name")]
        public string ServiceName { get; set; }
        [Display(Name = "Display name", Prompt = "Display name")]
        public string DisplayName { get; set; }
        public string Description { get; set; }
        [Display(Name = "Feed name", Prompt = "Feed name")]
        public ObjectId FeedId { get; set; }
        [Display(Name = "Assembly-qualified type name", Prompt = "Assembly-qualified type name")]
        public string AssemblyQualifiedType { get; set; }
        [Display(Name = "Package name", Prompt = "Package name")]
        public string PackageName { get; set; }

        readonly IEnumerable<KeyValuePair<ObjectId, string>> _allFeeds;

        public IEnumerable<KeyValuePair<ObjectId, string>> AllFeeds
        {
            get { return _allFeeds; }
        }

        public ServiceAppModel()
        {
            IsNew = true;
            Id = new ObjectId();
        }

        public ServiceAppModel(IEnumerable<KeyValuePair<ObjectId, string>> allFeeds)
        {
            IsNew = true;
            Id = new ObjectId();
            _allFeeds = allFeeds;
        }

        public ServiceAppModel(ServiceApp serviceApp, IEnumerable<KeyValuePair<ObjectId, string>> allFeeds)
        {
            Id = serviceApp.Id;

            DisplayName = serviceApp.DisplayName;
            ServiceName = serviceApp.ServiceName;
            AssemblyQualifiedType = serviceApp.AssemblyQualifiedType;
            Description = serviceApp.Description;
            FeedId = serviceApp.FeedId;
            PackageName = serviceApp.PackageName;
            _allFeeds = allFeeds;

            IsNew = false;
        }

        public void Update(ServiceApp serviceApp)
        {
            serviceApp.DisplayName = DisplayName;
            serviceApp.ServiceName = ServiceName;
            serviceApp.AssemblyQualifiedType = AssemblyQualifiedType;
            serviceApp.PackageName = PackageName;
            serviceApp.Description = Description;
            serviceApp.FeedId = FeedId;
        }

        public ServiceApp ToServiceApp()
        {
            var serviceApp = new ServiceApp
            {
                Id = Id
            };

            Update(serviceApp);
            return serviceApp;
        }
    }
}
