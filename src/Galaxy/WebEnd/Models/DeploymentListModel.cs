using System.Collections.Generic;
using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.Collections;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class DeploymentListModel 
    {
        public readonly DeploymentModel[] Deployments;
        public readonly KeyValuePair<ObjectId, string>[] AllFeeds;

        public DeploymentListModel(DashBoard dashBoard)
        {
            AllFeeds = dashBoard.Feeds.ConvertToArray(feed => new KeyValuePair<ObjectId, string>(feed.Id, feed.Name), dashBoard.Feeds.Count);
            Deployments = dashBoard.Deployments.ConvertToArray(x => new DeploymentModel(x, AllFeeds), dashBoard.Deployments.Count);
        }

        public int Count
        {
            get { return Deployments.Length; }
        }
    }
}