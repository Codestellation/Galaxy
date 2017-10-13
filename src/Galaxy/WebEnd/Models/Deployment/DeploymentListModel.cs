using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Quarks.Collections;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models.Deployment
{
    public class DeploymentListModel
    {
        public readonly DeploymentListItemModel[] Deployments;
        public readonly KeyValuePair<ObjectId, string>[] AllFeeds;

        public DeploymentListModel(NugetFeed[] feeds, DeploymentBoard deploymentBoard)
        {
            AllFeeds = feeds.ConvertToArray(feed => new KeyValuePair<ObjectId, string>(feed.Id, feed.Name));
            Deployments = deploymentBoard.Deployments.ConvertToArray(x => new DeploymentListItemModel(x, AllFeeds), deploymentBoard.Deployments.Count);

            Groups = Deployments
                .Select(GetGroup)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
        }

        public string[] Groups { get; set; }

        public int Count => Deployments.Length;

        public IEnumerable<DeploymentListItemModel> GetModelsByGroup(string serviceGroup)
        {
            var modelsByGroup = Deployments
                .Where(model => serviceGroup.Equals(GetGroup(model), StringComparison.Ordinal))
                .OrderBy(x => x.DisplayName);
            return modelsByGroup;
        }

        private static string GetGroup(DeploymentListItemModel model)
        {
            return string.IsNullOrWhiteSpace(model.Group) ? "Everything Else" : model.Group;
        }
    }
}