using System.Collections.Generic;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class DeploymentListModel 
    {
        public readonly DeploymentModel[] Deployments;
        public readonly KeyValuePair<ObjectId, string>[] AllFeeds;

        public DeploymentListModel(DashBoard dashBoard)
        {
            AllFeeds = dashBoard.Feeds.ConvertToArray(x => new KeyValuePair<ObjectId, string>(x.Id, x.Name));
            Deployments = dashBoard.Deployments.ConvertToArray(x => new DeploymentModel(x, AllFeeds));
        }

        public int Count
        {
            get { return Deployments.Length; }
        }
    }
}