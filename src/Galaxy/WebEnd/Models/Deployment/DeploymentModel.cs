using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models.Deployment
{
    public class DeploymentModel
    {
        public ObjectId Id { get; set; }

        public string Group { get; set; }
        public string DisplayName { get; set; }
        public string InstanceName { get; set; }

        public ObjectId FeedId { get; set; }
        public string PackageId { get; set; }
        public Version PackageVersion { get; set; }

        public string Status { get; set; }

        private readonly IEnumerable<KeyValuePair<Version, string>> _packageVersions;
        public IEnumerable<KeyValuePair<Version, string>> PackageVersions 
        {
            get { return _packageVersions; } 
        }

        private readonly IEnumerable<KeyValuePair<ObjectId, string>> _allFeeds;

        public string FeedName
        {
            get { return _allFeeds.Single(x => x.Key == FeedId).Value; }
        }

        public string State { get; set; }

        //used by nancy model binder
        public DeploymentModel()
        {
        }

        private DeploymentModel(Domain.Deployment deployment)
        {
            Id = deployment.Id;

            DisplayName = deployment.GetDisplayName();
            InstanceName = deployment.InstanceName;

            FeedId = deployment.FeedId;
            Status = deployment.Status;
            PackageVersion = deployment.PackageVersion;
            PackageId = deployment.PackageId;

            State = deployment.GetServiceState();

            Group = deployment.Group;
        }

        public DeploymentModel(Domain.Deployment deployment, 
            IEnumerable<KeyValuePair<ObjectId, string>> allFeeds,
            IEnumerable<Version> packageVersions):
            this(deployment)
        {
            _allFeeds = allFeeds;
            _packageVersions = packageVersions.OrderByDescending(x => x).Select(item => new KeyValuePair<Version, string>(item, item.ToString()));
        }
    }
}
