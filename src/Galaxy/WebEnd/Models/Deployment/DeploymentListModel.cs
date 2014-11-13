using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.Collections;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models.Deployment
{
    public class DeploymentListModel 
    {
        public readonly DeploymentListItemModel[] Deployments;
        public readonly KeyValuePair<ObjectId, string>[] AllFeeds;

        public DeploymentListModel(DashBoard dashBoard)
        {
            AllFeeds = dashBoard.Feeds.ConvertToArray(feed => new KeyValuePair<ObjectId, string>(feed.Id, feed.Name), dashBoard.Feeds.Count);
            Deployments = dashBoard.Deployments.ConvertToArray(x => new DeploymentListItemModel(x, AllFeeds), dashBoard.Deployments.Count);
            Groups = Deployments.Select(GetGroup).Distinct().OrderBy(x => x).ToArray();
        }

        public string[] Groups { get; set; }

        public int Count
        {
            get { return Deployments.Length; }
        }

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