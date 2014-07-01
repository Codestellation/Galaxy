using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nejdb.Bson;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class DeploymentModel
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
        public Version PackageVersion { get; set; }

        public string Status { get; set; }

        readonly IEnumerable<KeyValuePair<Version, string>> _packageVersions;
        public IEnumerable<KeyValuePair<Version, string>> PackageVersions 
        {
            get { return _packageVersions; } 
        }

        readonly IEnumerable<KeyValuePair<ObjectId, string>> _allFeeds;
        public IEnumerable<KeyValuePair<ObjectId, string>> AllFeeds
        {
            get { return _allFeeds; }
        }

        public string FeedName
        {
            get { return _allFeeds.Single(x => x.Key == FeedId).Value; }
        }

        public DeploymentModel()
        {
            IsNew = true;
            Id = new ObjectId();
            _allFeeds = null;
            _packageVersions = null;
        }

        public DeploymentModel(IEnumerable<KeyValuePair<ObjectId, string>> allFeeds)
        {
            IsNew = true;
            Id = new ObjectId();
            _allFeeds = allFeeds;
            _packageVersions = null;
        }

        public DeploymentModel(Deployment deployment)
        {
            Id = deployment.Id;

            DisplayName = deployment.DisplayName;
            ServiceName = deployment.ServiceName;
            AssemblyQualifiedType = deployment.AssemblyQualifiedType;
            Description = deployment.Description;
            FeedId = deployment.FeedId;
            Status = deployment.Status;
            PackageVersion = deployment.PackageVersion;
            PackageName = deployment.PackageName;

            IsNew = false;
        }

        public DeploymentModel(Deployment deployment,
            IEnumerable<KeyValuePair<ObjectId, string>> allFeeds):
            this(deployment)
        {
            _allFeeds = allFeeds;
            _packageVersions = null;
        }

        public DeploymentModel(Deployment deployment, 
            IEnumerable<KeyValuePair<ObjectId, string>> allFeeds,
            IEnumerable<Version> packageVersions):
            this(deployment)
        {
            _allFeeds = allFeeds;
            _packageVersions = packageVersions.Select(item => new KeyValuePair<Version, string>(item, item.ToString()));
        }

        public void Update(Deployment deployment)
        {
            deployment.DisplayName = DisplayName;
            deployment.ServiceName = ServiceName;
            deployment.AssemblyQualifiedType = AssemblyQualifiedType;
            deployment.Description = Description;
            deployment.FeedId = FeedId;
            deployment.PackageName = PackageName;
        }

        public Deployment ToDeployment()
        {
            var deployment = new Deployment
            {
                Id = Id
            };

            Update(deployment);
            return deployment;
        }
    }
}
