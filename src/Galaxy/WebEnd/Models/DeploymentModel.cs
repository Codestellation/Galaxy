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

        [Display(Name = "Instance name", Prompt = "Instance name")]
        public string InstanceName { get; set; }

        public string Description { get; set; }
        [Display(Name = "Feed", Prompt = "<Select feed>")]
        public ObjectId FeedId { get; set; }
        [Display(Name = "Assembly-qualified type name", Prompt = "Assembly-qualified type name")]
        public string AssemblyQualifiedType { get; set; }
        [Display(Name = "Package", Prompt = "Package Id")]
        public string PackageId { get; set; }
        [Display(Prompt = "<Select version>")]
        public Version PackageVersion { get; set; }
        public string ConfigFileContent { get; set; }
        public string Status { get; set; }

        [Display(Name = "Keep on update", Prompt = "files, folders, allowed file masks")]
        public string KeepOnUpdate { get; set; }

        private readonly IEnumerable<KeyValuePair<Version, string>> _packageVersions;
        public IEnumerable<KeyValuePair<Version, string>> PackageVersions 
        {
            get { return _packageVersions; } 
        }

        private readonly IEnumerable<KeyValuePair<ObjectId, string>> _allFeeds;
        public IEnumerable<KeyValuePair<ObjectId, string>> AllFeeds
        {
            get { return _allFeeds; }
        }

        public string FeedName
        {
            get { return _allFeeds.Single(x => x.Key == FeedId).Value; }
        }

        //used by nancy model binder
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

        private DeploymentModel(Deployment deployment)
        {
            Id = deployment.Id;

            AssemblyQualifiedType = deployment.AssemblyQualifiedType;
            DisplayName = deployment.DisplayName;
            ServiceName = deployment.ServiceName;
            InstanceName = deployment.InstanceName;
            Description = deployment.Description;

            FeedId = deployment.FeedId;
            Status = deployment.Status;
            PackageVersion = deployment.PackageVersion;
            PackageId = deployment.PackageId;
            
            ConfigFileContent = deployment.ConfigFileContent;
            KeepOnUpdate = (deployment.KeepOnUpdate ?? new FileList(new string[0])).ToString();

            IsNew = false;
        }

        public DeploymentModel(Deployment deployment, IEnumerable<KeyValuePair<ObjectId, string>> allFeeds):
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
            deployment.AssemblyQualifiedType = AssemblyQualifiedType;
            deployment.DisplayName = DisplayName;
            deployment.ServiceName = ServiceName;
            deployment.InstanceName = InstanceName;
            deployment.Description = Description;
            deployment.FeedId = FeedId;
            deployment.PackageId = PackageId;

            deployment.KeepOnUpdate = ParseKeepOnUpdate();
        }

        private FileList ParseKeepOnUpdate()
        {
            if (string.IsNullOrWhiteSpace(KeepOnUpdate))
            {
                return new FileList(new string[0]);
            }

            var patterns = KeepOnUpdate
                .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();

            return new FileList(patterns);
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
