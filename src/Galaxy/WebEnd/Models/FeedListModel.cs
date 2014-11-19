using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Quarks.Collections;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class FeedListModel 
    {
        private FeedModel[] _feeds;

        public FeedListModel(FeedBoard feedBoard, DeploymentBoard deploymentBoard)
        {
            _feeds = feedBoard.Feeds.ConvertToArray(x =>
            {
                var inUse = deploymentBoard.Deployments.Any(deployment => deployment.FeedId.Equals(x.Id));
                return new FeedModel(x, inUse);
            }, feedBoard.Feeds.Count);
        }

        public FeedModel[] Feeds
        {
            get { return _feeds; }
        }

        public int FeedCount
        {
            get { return _feeds.Length; }
        }
    }
}