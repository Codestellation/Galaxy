using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Nejdb.Bson;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class DeploymentModel
    {
        public ObjectId Id { get; set; }
        public bool IsNew { get; set; }

        public string DisplayName { get; set; }

        [Display(Name = "Service group", Prompt = "Service group")]
        public string Group { get; set; }

        [Display(Name = "Instance name", Prompt = "Instance name")]
        public string InstanceName { get; set; }

        [Display(Name = "Feed", Prompt = "<Select feed>")]
        public ObjectId FeedId { get; set; }
        [Display(Name = "Package", Prompt = "Package Id")]
        public string PackageId { get; set; }
        [Display(Prompt = "<Select version>")]
        public Version PackageVersion { get; set; }

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

        public string State { get; set; }

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

            DisplayName = deployment.GetDisplayName();
            InstanceName = deployment.InstanceName;

            FeedId = deployment.FeedId;
            Status = deployment.Status;
            PackageVersion = deployment.PackageVersion;
            PackageId = deployment.PackageId;

            KeepOnUpdate = (deployment.KeepOnUpdate ?? new FileList(new string[0])).ToString();

            State = deployment.GetServiceState();

            Group = deployment.Group;

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
            _packageVersions = packageVersions.OrderByDescending(x => x).Select(item => new KeyValuePair<Version, string>(item, item.ToString()));
        }

        public void Update(Deployment deployment)
        {
            deployment.InstanceName = InstanceName;
            deployment.FeedId = FeedId;
            deployment.PackageId = PackageId;
            deployment.Group = Group;
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
