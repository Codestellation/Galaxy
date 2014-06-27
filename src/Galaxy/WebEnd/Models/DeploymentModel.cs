﻿using System.Collections.Generic;
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
        public string Status { get; set; }

        readonly IEnumerable<KeyValuePair<ObjectId, string>> _allFeeds;

        public IEnumerable<KeyValuePair<ObjectId, string>> AllFeeds
        {
            get { return _allFeeds; }
        }

        public DeploymentModel()
        {
            IsNew = true;
            Id = new ObjectId();
        }

        public DeploymentModel(IEnumerable<KeyValuePair<ObjectId, string>> allFeeds)
        {
            IsNew = true;
            Id = new ObjectId();
            _allFeeds = allFeeds;
        }

        public DeploymentModel(Deployment deployment, IEnumerable<KeyValuePair<ObjectId, string>> allFeeds)
        {
            Id = deployment.Id;

            DisplayName = deployment.DisplayName;
            ServiceName = deployment.ServiceName;
            AssemblyQualifiedType = deployment.AssemblyQualifiedType;
            Description = deployment.Description;
            FeedId = deployment.FeedId;
            Status = deployment.Status;

            _allFeeds = allFeeds;

            IsNew = false;
        }

        public void Update(Deployment deployment)
        {
            deployment.DisplayName = DisplayName;
            deployment.ServiceName = ServiceName;
            deployment.AssemblyQualifiedType = AssemblyQualifiedType;
            deployment.Description = Description;
            deployment.FeedId = FeedId;
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